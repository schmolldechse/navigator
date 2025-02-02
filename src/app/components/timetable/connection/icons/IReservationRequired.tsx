interface Props {
    width?: number;
    height?: number;
}

const IReservationRequired = ({ width = 25, height = 25 }: Props) => {
    return (
        <svg
            id="reservation-required"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            height={height}
            width={width}
        >
            <rect fill={"none"} stroke={"#fff"} strokeMiterlimit={"10"} strokeWidth={"10x"} x="48.14" y="62.93" width="945.91" height="945.91" rx="50" ry="50"/>
            <text fill={"#fff"} fontFamily={"Calibri, Calibri"} fontSize={"1000px"} transform="translate(247.56 853.33)">
                <tspan x="0" y="0">R</tspan>
            </text>
        </svg>
    );
};

export default IReservationRequired;
