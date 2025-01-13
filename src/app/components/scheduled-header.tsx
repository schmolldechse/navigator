interface HeaderProps {
    isDeparture: boolean
}

const ScheduledHeader: React.FC<HeaderProps> = ({isDeparture}) => {
    return (<>
        {/* layout for smaller screens (under md) */}
        <div className="container mx-4 flex justify-between text-sm md:hidden">
            <p className="text-left">Zug / Bus</p>
            <p className="text-right">Gleis</p>
        </div>

        {/* layout for greater screens (above md) */}
        <div className="container mx-auto hidden md:flex justify-between text-base">
            <div className="flex-[1] text-right mr-8">
                <p>Zug / Bus</p>
                <p>Zeit</p>
            </div>

            <div className="flex-[4] text-left mr-4">
                <p>Über</p>
                <p>{isDeparture ? 'Ziel' : 'Herkunft'}</p>
            </div>

            <div className="flex-[1] flex justify-end items-end text-right">
                <p>Gleis</p>
            </div>
        </div>
    </>)
}

export default ScheduledHeader;
