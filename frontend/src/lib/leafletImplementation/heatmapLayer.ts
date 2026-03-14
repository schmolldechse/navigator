import { DomUtil, Layer, Map } from "leaflet";
import { interpolateTurbo } from "d3-scale-chromatic";

type ColorScale = (t: number) => string;

// [latitude, longitude, intensity]
type HeatPoint = [number, number, number];

type HeatmapOptions = {
	radius?: number;
	blur?: number;
	maxIntensity?: number;
	gradient?: ColorScale;
};

/**
 * A heatmap layer for Leaflet v2 using custom canvas rendering.
 * Renders weighted lat/lng points as a heatmap overlay using radial gradients.
 */
class HeatmapLayer extends Layer {
	private _canvas: HTMLCanvasElement | null = null;
	private _ctx: CanvasRenderingContext2D | null = null;
	private _data: HeatPoint[] = [];
	private _palette: Uint8ClampedArray | null = null;
	private _circle: HTMLCanvasElement | null = null;

	private _radius: number;
	private _blur: number;
	private _maxIntensity: number;
	private _gradient: ColorScale;

	constructor(data: HeatPoint[] = [], options: HeatmapOptions = {}) {
		super();
		this._data = data;
		this._radius = options.radius ?? 25;
		this._blur = options.blur ?? 15;
		this._maxIntensity = options.maxIntensity ?? 0;
		this._gradient = options.gradient ?? interpolateTurbo;
	}

	onAdd(map: Map): this {
		this._canvas = DomUtil.create("canvas", "leaflet-heatmap-layer") as HTMLCanvasElement;

		const pane = this.getPane();
		if (pane) pane.appendChild(this._canvas);

		this._ctx = this._canvas.getContext("2d");

		this._canvas.style.position = "absolute";
		this._canvas.style.pointerEvents = "none";

		this._buildPalette();
		this._buildCircleBrush();

		map.on("moveend", this._redraw, this);
		map.on("resize", this._redraw, this);

		this._redraw();
		return this;
	}

	onRemove(map: Map): this {
		map.off("moveend", this._redraw, this);
		map.off("resize", this._redraw, this);

		if (this._canvas && this._canvas.parentNode) this._canvas.parentNode.removeChild(this._canvas);

		this._canvas = null;
		this._ctx = null;
		return this;
	}

	setData(data: HeatPoint[]): this {
		this._data = data;
		this._redraw();
		return this;
	}

	/** Build a 256-color palette from the gradient stops */
	private _buildPalette(): void {
		const paletteCanvas = document.createElement("canvas");
		paletteCanvas.width = 256;
		paletteCanvas.height = 1;

		const ctx = paletteCanvas.getContext("2d")!;

		const grad = ctx.createLinearGradient(0, 0, 256, 0);
		for (let i = 0; i <= 20; i++) {
			grad.addColorStop(i / 20, this._gradient(i / 20));
		}

		ctx.fillStyle = grad;
		ctx.fillRect(0, 0, 256, 1);

		this._palette = ctx.getImageData(0, 0, 256, 1).data as unknown as Uint8ClampedArray;
	}

	/** Build a greyscale radial-gradient circle brush */
	private _buildCircleBrush(): void {
		const r = this._radius + this._blur;

		const circle = (this._circle = document.createElement("canvas"));
		circle.width = circle.height = r * 2;

		const ctx = circle.getContext("2d")!;
		ctx.shadowOffsetX = ctx.shadowOffsetY = r * 2;
		ctx.shadowBlur = this._blur;
		ctx.shadowColor = "black";

		ctx.beginPath();
		ctx.arc(-r, -r, this._radius, 0, Math.PI * 2, true);
		ctx.closePath();
		ctx.fill();
	}

	private _redraw(): void {
		const map = this._map;
		if (!map || !this._canvas || !this._ctx || !this._circle || !this._palette) return;

		const size = map.getSize();
		if (size.x === 0 || size.y === 0) return;

		const topLeft = map.containerPointToLayerPoint([0, 0]);

		DomUtil.setPosition(this._canvas, topLeft);
		this._canvas.width = size.x;
		this._canvas.height = size.y;

		const ctx = this._ctx;
		ctx.clearRect(0, 0, size.x, size.y);

		if (this._data.length === 0) return;

		// Determine max intensity for normalization
		let maxVal = this._maxIntensity;
		if (maxVal === 0) {
			for (const point of this._data) {
				if (point[2] > maxVal) maxVal = point[2];
			}
		}
		if (maxVal === 0) maxVal = 1;

		const brushRadius = this._radius + this._blur;

		// Draw greyscale alpha circles
		for (const [lat, lng, intensity] of this._data) {
			const point = map.latLngToContainerPoint([lat, lng]);
			ctx.globalAlpha = Math.min(1, Math.max(0.05, intensity / maxVal));
			ctx.drawImage(this._circle, point.x - brushRadius, point.y - brushRadius);
		}

		// Colorize the greyscale canvas using the palette
		const imageData = ctx.getImageData(0, 0, size.x, size.y);
		const pixels = imageData.data;
		const palette = this._palette;

		for (let i = 0; i < pixels.length; i += 4) {
			const alpha = pixels[i + 3];
			if (alpha === 0) continue;

			const paletteIdx = Math.min(255, alpha) * 4;
			pixels[i] = palette[paletteIdx]; // R
			pixels[i + 1] = palette[paletteIdx + 1]; // G
			pixels[i + 2] = palette[paletteIdx + 2]; // B
			pixels[i + 3] = Math.min(255, alpha * 2); // boost visibility
		}

		ctx.putImageData(imageData, 0, 0);
	}
}

export { HeatmapLayer, type HeatPoint, type HeatmapOptions };
