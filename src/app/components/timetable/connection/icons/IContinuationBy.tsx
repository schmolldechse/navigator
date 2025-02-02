interface Props {
    width?: number;
    height?: number;
}

const IContinuationBy = ({width = 25, height = 25}: Props) => {
    return (
        <svg
            id="continuation-by"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            height={height}
            width={width}
            className={"shrink-0"}
        >
            <rect fill={"none"} stroke={"#fff"} strokeMiterlimit={"10"} strokeWidth={"30px"} x="39.04" y="39.04" width="945.91"
                  height="945.91" rx="50" ry="50"/>
            <g>
                <circle fill={"#fff"} cx="511.96" cy="266.78" r="95.23"/>
                <polygon fill={"#fff"}
                         points="435.3 448.22 435.3 776.15 371.83 775.73 371.83 852.44 652.13 852.44 652.13 781.84 588.66 781.43 588.66 405.87 371.83 405.87 371.83 448.22 435.3 448.22"/>
            </g>
        </svg>
    );
};

export default IContinuationBy;
