interface Props {
    width?: number;
    height?: number;
}

const IUnplannedInfo = ({ width = 25, height = 25 }: Props) => {
    return (
        <svg
            id="unplanned-info"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            height={height}
            width={width}
            className={"ml-1"}
        >
            <circle fill={"#0a0a0a"} cx="511.96" cy="202.33" r="123.1"/>
            <polygon fill={"#0a0a0a"} points="412.86 436.87 412.86 860.79 330.81 860.25 330.81 959.42 693.16 959.42 693.16 868.14 611.11 867.62 611.11 382.12 330.81 382.12 330.81 436.87 412.86 436.87"/>
        </svg>
    )
}

export default IUnplannedInfo;
