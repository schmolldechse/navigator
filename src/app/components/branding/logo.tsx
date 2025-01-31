import Image from "next/image";

interface Props {
    onClick?: () => void;
}

const Logo = ({ onClick }: Props) => {
    return (
        <span className={"flex flex-row items-center montserrat-bold"} onClick={onClick}>
            <Image className={"ml-[10px]"} width={75} height={75} src={"/branding/logo.svg"} alt={"LOGO"} />
            <h1 className={"hidden md:inline text-[2rem] relative top-[-0.25rem]"}>NAVIGATOR</h1>
        </span>
    );
}

export default Logo;
