interface Props {
    width?: number;
    height?: number;
}

const IReservationRequired = ({width = 25, height = 25}: Props) => {
    return (
        <svg
            id="reservation-required"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            height={height}
            width={width}
            className={"shrink-0"}
        >
            <rect fill={"none"} stroke={"#fff"} strokeMiterlimit={"10"} strokeWidth={"30px"} x="48.14" y="62.93" width="945.91" height="945.91" rx="50" ry="50"/>
            <path fill={"#fff"}
                  d="M596.44,589.68c71.39-21.38,123.7-69.87,128.23-186.16-2.29-129.95-72.52-193.21-204.27-196.1h-235.39v656.94h97.1v-264.75h109.91l155.7,264.75h111.76l-163.05-274.68ZM382.11,520.99v-229.52h139.19c71.29.86,104.82,41.09,106.26,114.76-.41,73.19-34.46,112.81-106.26,114.76h-139.19Z"/>
        </svg>
    );
};

export default IReservationRequired;
