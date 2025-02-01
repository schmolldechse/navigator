import React from "react";

interface Props {
    width?: number;
    height?: number;
}

const IAdditionalStops = ({ width = 25, height = 25 }: Props) => {
    return (
        <svg
            id="canceled-stops"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            width={width}
            height={height}
            className={"ml-1"}
        >
            <rect fill={"#fff"} x="-413.71" y="-331.84" width="1851.43" height="1598.24"/>
            <text letterSpacing={".12em"} fill={"#0a0a0a"} fontFamily={"Calibri-Bold, Calibri"} fontWeight={"700"} fontSize={"1200px"} transform="translate(66.87 913.75) scale(.75 1)">
                <tspan x="0" y="0">H</tspan>
            </text>
            <text fill={"#0a0a0a"} fontFamily={"Calibri-Bold, Calibri"} fontWeight={"700"} fontSize={"1200px"} letterSpacing={".12em"} transform="translate(531.14 913.75) scale(.8 1)">
                <tspan x="0" y="0"></tspan>
            </text>
            <text fill={"#0a0a0a"} fontFamily={"Calibri-Bold, Calibri"} fontWeight={"700"} fontSize={"850px"} letterSpacing={"-.12em"} transform="translate(637.77 538.75) scale(.8)">
                <tspan x="0" y="0">+</tspan>
            </text>
        </svg>
    );
};

export default IAdditionalStops;
