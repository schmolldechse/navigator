interface HeaderProps {
    isDeparture: boolean
}

const ScheduledHeader: React.FC<HeaderProps> = ({ isDeparture }) => {
    return (
        <div className="container mx-auto flex justify-between space-x-4 text-white text-base">
            {/* First col */}
            <div className="flex-[1] text-right mr-8">
                <p>Zug / Bus</p>
                <p>Zeit</p>
            </div>

            {/* Second col */}
            <div className="flex-[4] text-left">
                <p>Über</p>
                <p>{isDeparture ? 'Ziel' : 'Herkunft'}</p>
            </div>

            {/* Third col */}
            <div className="flex-[1] flex justify-end items-end text-right">
                <p>Gleis</p>
            </div>
        </div>
    )
}

export default ScheduledHeader;
