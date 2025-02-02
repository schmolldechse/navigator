interface Props {
    width?: number;
    height?: number;
}

const IReservationsMissing = ({width = 25, height = 25}: Props) => {
    return (
        <svg
            id="reservations-missing"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            height={height}
            width={width}
            className={"shrink-0"}
        >
            <rect fill={"#fff"} x="-149.86" y="-130.87" width="1300.75" height="1357.71"/>
            <polygon fill={"#0a0a0a"} points="807.65 92.95 150.12 843.99 250.12 843.99 907.88 92.95 807.65 92.95"/>
            <g>
                <path fill={"#0a0a0a"}
                      d="M388.87,535.09v-2.89h2.53l68.84-78.63h-71.37v-229.52h139.19c45.18.54,75.19,16.91,91.48,47.57l65.09-74.35c-34.29-36.95-87.11-55.73-157.47-57.27h-235.39v506l97.1-110.91Z"/>
                <polygon fill={"#0a0a0a"} points="335.58 796.94 388.87 796.94 388.87 736.11 335.58 796.94"/>
                <path fill={"#0a0a0a"}
                      d="M730.93,345.53l-204.57,233.57,128.11,217.84h111.76l-163.05-274.68c69.43-20.79,120.8-67.23,127.74-176.73Z"/>
            </g>
        </svg>
    );
};

export default IReservationsMissing;
