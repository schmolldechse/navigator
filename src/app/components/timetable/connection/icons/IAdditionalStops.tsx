interface Props {
    width?: number;
    height?: number;
}

const IAdditionalStops = ({width = 25, height = 25}: Props) => {
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
            <polygon fill={"#0a0a0a"}
                     points="927.77 399.36 809.42 399.36 809.42 281.02 719.35 281.02 719.35 399.36 601 399.36 601 489.44 719.35 489.44 719.35 607.79 809.42 607.79 809.42 489.44 927.77 489.44 927.77 399.36"/>
        </svg>
    );
};

export default IAdditionalStops;
