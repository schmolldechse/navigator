import React from "react";

interface Props {
    width?: number;
    height?: number;
}

const IBicycleReservationRequired = ({ width = 25, height = 25 }: Props) => {
    return (
        <svg
            id="bicycle-reservation-required"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            width={width}
            height={height}
        >
            <g>
                <circle strokeWidth={"50px"} fill={"none"} stroke={"#fff"} strokeMiterlimit={"10"}  cx="701.47" cy="624.19" r="130.48"/>
                <path fill={"#fff"}
                      d="M1175.89,621.29c1.4,3.72,3.43,7.2,6.13,10.11,4.27,4.61,10.88,8.06,20.54,5.85,12.66-2.9,18.46-15.56,11.21-37.52l-108.79-293.41s-15.3-37.45-53.8-37.45h-60.13v46.95l67.52.53,30.07,72.79,87.26,232.15Z"/>
                <path fill={"#fff"}
                      d="M756.73,346.1c-4.25,0-8.42-1.39-12.01-3.66-4.27-2.69-8.62-7.3-8.96-15.02-.43-9.56-1.51-16.6-.18-23.29s6.87-10.99,13.38-10.99h119.2s16.62,0,15.3,16.88c0,0,2.37,16.48-40.09,25.85,0,0-45.63,10.25-86.63,10.24Z"/>
                <polygon fill={"#fff"} points="778.25 337.31 902.21 607.12 942.95 580.48 821.5 325.45 778.25 337.31"/>
                <path fill={"#fff"}
                      d="M805.81,370.94l-112.83,218.73c-3.83,7.42-6.5,15.21-8.9,23.34-3.09,10.46-6.58,21.28,1.86,30.25,6.54,6.95,12.99,7.58,18.23,6.1,4.93-1.39,8.99-4.88,11.42-9.39l118.7-220.36-28.48-48.66Z"/>
                <path fill={"#fff"}
                      d="M864.53,442.97l222.07-88.09,7.65,31.25c12.97,13.13-223.28,115.6-220.22,99.43l-9.49-42.59Z"/>
                <path fill={"#fff"}
                      d="M1100.31,406.45c-7.85,1.14-155.13,157.98-181.77,190l-16.03,3.85h-53.01l-2.77,49.71h49.74c13.54,0,26.81-3.9,38.14-11.32,8.39-5.5,18.61-13.22,29.22-23.62,40.62-39.82,100-104.36,150.07-172.48l-13.58-36.13Z"/>
            </g>
            <rect strokeWidth={"30px"} fill={"none"} stroke={"#fff"} strokeMiterlimit={"10"} x="43.92" y="38.81" width="945.91" height="945.91" rx="50" ry="50"/>
            <text fill={"#fff"} fontFamily={"Calibri, Calibri"} fontSize={"779.2px"} transform="translate(61.7 754.67)">
                <tspan x="0" y="0">R</tspan>
            </text>
        </svg>
    );
};

export default IBicycleReservationRequired;
