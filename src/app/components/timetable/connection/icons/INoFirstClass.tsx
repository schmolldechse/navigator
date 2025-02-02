interface Props {
    width?: number;
    height?: number;
}

const INoFirstClass = ({ width = 25, height = 25 }: Props) => {
    return (
        <svg
            id="no-first-class"
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 1024 1024"
            height={height}
            width={width}
        >
            <path fill={"#0a0a0a"}
                  d="M227.95,393.08l-.13-140.8c85.85-11.03,146-41.99,176.74-96.06h167.49v223.65L845.55,67.27h111.18L178.45,956.73h-111.18l306.86-350.71.13-256.34h-3.91c-38.9,25.72-86.68,39.78-142.4,43.41Z"/>
            <polygon fill={"#0a0a0a"}
                     points="322.99 867.78 572.04 583.16 572.04 747.71 667.52 747.71 667.52 867.78 322.99 867.78"/>
            <rect fill={"#0a0a0a"} x="778.84" y="734.36" width="133.42" height="133.42"/>
        </svg>
    );
};

export default INoFirstClass;
