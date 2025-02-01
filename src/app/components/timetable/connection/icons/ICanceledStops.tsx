import React from "react";

interface Props {
    width?: number;
    height?: number;
}

const ICanceledStops = ({ width = 25, height = 25 }: Props) => {
    return (
        <svg
            id="canceled-stops"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            width={width}
            height={height}
            className={"ml-1"}
        >
            <rect fill={"#fff"} x="-393.55" y="-339.75" width="1851.43" height="1598.24"/>
            <text letterSpacing={"-.1em"} fill={"#0a0a0a"} fontFamily={"Calibri-Bold, Calibri"} fontWeight={"700"} fontSize={"1200px"} transform="translate(87.03 905.83) scale(.75 1)">
                <tspan x="0" y="0">H</tspan>
            </text>
            <text letterSpacing={"-.1em"} fontSize={"1200px"} fill={"#0a0a0a"} fontFamily={"Calibri-Bold, Calibri"} fontWeight={"700"} transform="translate(564.81 905.83) scale(.8 1)">
                <tspan x="0" y="0"></tspan>
            </text>
            <text fontSize={"1000px"} fill={"#0a0a0a"} fontFamily={"Calibri-Bold, Calibri"} fontWeight={"700"} letterSpacing={"-.1em"} transform="translate(685.84 555.83)">
                <tspan x="0" y="0">-</tspan>
            </text>
        </svg>
    );
};

export default ICanceledStops;
