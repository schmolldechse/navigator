interface Props {
    onClick: () => void;
}

const CShowMore = ({onClick}: Props) => {
    return (
        <span className="ml-2 inline-flex items-center justify-center cursor-pointer px-2 py-1 rounded-2xl bg-gray-700 group"
              onClick={onClick} style={{verticalAlign: 'middle'}}
        >
            <svg width="16" height="3" xmlns="http://www.w3.org/2000/svg"
                 className="transition-all duration-300 group-hover:w-[22px]"
            >
                <circle cx="2" cy="1.5" r="1.5" fill="white"/>
                <circle cx="8" cy="1.5" r="1.5" fill="white"/>
                <circle cx="14" cy="1.5" r="1.5" fill="white"/>

                <circle cx="20" cy="1.5" r="1.5" fill="white"
                        className="scale-0 translate-x-[-6px] opacity-0 transition-all duration-300 origin-left group-hover:scale-100 group-hover:translate-x-0 group-hover:opacity-100"
                />
            </svg>
        </span>
    );
};

export default CShowMore;
