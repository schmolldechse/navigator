interface Props {
    width?: number;
    height?: number;
}

const ITrackChanged = ({ width = 25, height = 25 }: Props) => {
    return (
        <svg
            id="track-changed"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            height={ height }
            width={ width }
        >
            <rect fill={"#fff"} x="-113.05" y="-113.05" width="1250.11" height="1250.11" />
            <g>
                <polygon fillRule={"evenodd"} fill={"#0a0a0a"} points="166.63 692.78 166.63 649.88 244.01 649.88 265.55 692.96 166.63 692.78" />
                <polygon fillRule={"evenodd"} fill={"#0a0a0a"} points="308.64 649.88 330.18 692.96 437.9 692.96 416.35 649.88 308.64 649.88" />
                <polygon fillRule={"evenodd"} fill={"#0a0a0a"} points="502.53 305.18 480.98 262.1 588.7 262.1 610.24 305.18 502.53 305.18" />
                <polygon fillRule={"evenodd"} fill={"#0a0a0a"} points="674.87 305.18 653.33 262.1 761.05 262.1 782.59 305.18 674.87 305.18" />
                <path fill={"#0a0a0a"}
                      d="M748.12,512l-53.86,53.86,99.1,90.48-47.4-4.31h-148.65l-205.31-410.61v-.02c-6.21-12.4-18.88-20.23-32.75-20.23h-192.81v86.17h161.57l205.35,410.61v.02c6.21,12.4,18.88,20.23,32.75,20.23h179.84l47.4-4.31-99.1,90.48,53.86,53.86,193.89-183.12-193.89-183.12Z"
                />
            </g>
        </svg>
    )
}

export default ITrackChanged;
