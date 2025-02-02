interface Props {
    width?: number;
    height?: number;
}

const INoOnwardJourney = ({width = 25, height = 25}: Props) => {
    return (
        <svg
            id="no-onward-journey"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            height={height}
            width={width}
            className={"shrink-0"}
        >
            <rect fill={"#fff"} x="-212.75" y="-192.58" width="1449.49" height="1338.73"/>
            <polygon fill={"#0a0a0a"}
                     points="848.93 236.12 752.66 139.85 512 380.52 271.34 139.85 175.07 236.12 415.74 476.78 175.07 717.44 271.34 813.71 512 573.04 752.66 813.71 848.93 717.44 608.26 476.78 848.93 236.12"/>
        </svg>
    );
};

export default INoOnwardJourney;
