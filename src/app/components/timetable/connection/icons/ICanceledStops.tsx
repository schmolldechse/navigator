interface Props {
    width?: number;
    height?: number;
}

const ICanceledStops = ({width = 25, height = 25}: Props) => {
    return (
        <svg
            id="canceled-stops"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            width={width}
            height={height}
            className={"shrink-0"}
        >
            <rect fill={"#fff"} x="-41.77" y="-41.77" width="1107.55" height="1107.55"/>
            <polygon fill={"#0a0a0a"}
                     points="376.79 238.63 376.79 455.42 241.79 455.42 241.79 238.63 117.78 238.63 117.78 782.19 241.79 782.19 241.79 567.77 376.79 567.77 376.79 782.19 500.81 782.19 500.81 238.63 376.79 238.63"/>
            <rect fill={"#0a0a0a"} x="601" y="399.36" width="326.77" height="90.07"/>
        </svg>
    );
};

export default ICanceledStops;
