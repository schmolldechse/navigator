import Image from "next/image";

const Logo: React.FC = ({ }) => {
    return (
        <span className={"flex flex-row items-center montserrat-bold"}>
            <Image className={"ml-[10px]"} width={75} height={75} src={"/branding/logo.svg"} alt={"LOGO"} />
            <h1 className={"hidden md:inline text-[2rem] relative top-[-0.25rem]"}>NAVIGATOR</h1>
        </span>
    );
}

export default Logo;
