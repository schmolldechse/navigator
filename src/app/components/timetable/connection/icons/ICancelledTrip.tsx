interface Props {
    width?: number;
    height?: number;
}

const ICancelledTrip = ({ width = 25, height = 25 }: Props) => {
    return (
        <svg
            id="cancelled-trip"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            height={height}
            width={width}
            className={"ml-1"}
        >
            <rect fill={"#0a0a0a"} x="-232.15" y="-167.22" width="1449.49" height="1338.73"/>
            <g>
                <rect fill={"#fff"} x="84.18" y="434.07" width="816.83" height="136.14" transform="translate(499.35 -201.24) rotate(45)"/>
                <rect fill={"#fff"} x="84.18" y="434.07" width="816.83" height="136.14" transform="translate(1195.98 508.9) rotate(135)"/>
            </g>
        </svg>
    )
}

export default ICancelledTrip;
