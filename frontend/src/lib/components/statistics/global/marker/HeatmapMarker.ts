import { DivIcon, Map, Marker, type LatLngExpression, type MarkerOptions } from "leaflet";
import { mount, unmount, type Component, type ComponentProps } from "svelte";

class HeatmapMarker<T extends Component<ComponentProps<T>>> extends Marker {
	private instance: ReturnType<typeof mount> | null = null;

	private component: T;
	private props: ComponentProps<T>;

	constructor(latlng: LatLngExpression, component: T, props: ComponentProps<T>, options?: MarkerOptions) {
		super(latlng, {
			...options,
			icon: new DivIcon({
				className: "heatmap-marker",
				iconSize: [32, 32],
				iconAnchor: [16, 16]
			})
		});

		this.component = component;
		this.props = props;
	}

	onAdd(map: Map) {
		super.onAdd(map);

		const element = this.getElement();
		if (!element) return this;

		this.instance = mount(this.component, {
			target: element,
			props: this.props
		});

		return this;
	}

	onRemove(map: Map) {
		if (this.instance) {
			unmount(this.instance);
			this.instance = null;
		}

		return super.onRemove(map);
	}
}

export { HeatmapMarker };
