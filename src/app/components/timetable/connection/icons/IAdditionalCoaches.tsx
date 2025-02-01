interface Props {
    width?: number;
    height?: number;
}

const IAdditionalCoaches = ({ width = 25, height = 25 }: Props) => {
    return (
        <svg
            id="additional-coaches"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            height={height}
            width={width}
        >
            <rect fill={"#fff"} x="603.59" y="179.99" width="332.01" height="80.14"/>
            <path fill={"#fff"}
                  d="M879.15,443.31H144.85c-24.85,0-45,20.15-45,45v298.46h45.75l.03,47.25c0,5.52,4.48,9.99,10,9.99h163.15c5.52,0,10-4.48,10-10v-47.24h366.36l-.36,47.17c-.04,5.55,4.45,10.08,10,10.08h163.54c5.52,0,10-4.48,10-10v-47.24h45.84v-298.46c0-24.85-20.15-45-45-45ZM832.56,636.88c0,6.9-5.6,12.5-12.5,12.5H203.94c-6.9,0-12.5-5.6-12.5-12.5v-112.38c0-6.9,5.6-12.5,12.5-12.5h616.13c6.9,0,12.5,5.6,12.5,12.5v112.38Z"/>
            <rect fill={"#fff"} x={"729.52"} y={"54.05"} width={"80.14"} height={"322.01"} />
        </svg>
    );
};

export default IAdditionalCoaches;
