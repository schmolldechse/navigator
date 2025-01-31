'use client';

import { usePathname, useRouter } from "next/navigation";
import Logo from "../branding/logo";

interface Props {
    id: string
}

const TNavbar: React.FC<Props> = ({ id }) => {
    const router = useRouter();
    const pathname = usePathname();

    const navbarItems = [
        { label: "Departures", path: '/departures' },
        { label: "Arrivals", path: '/arrivals' },
    ]

    const handleNavigation = (path: string) => router.push(`/${id}${path}`);

    return (
        <nav className="flex justify-between items-center text-xl montserrat-regular px-2 w-full">
            {/* home */}
            <Logo onClick={() => router.push("/")} />

            {/* navigation */}
            <div className="flex space-x-5">
                {navbarItems.map(({ label, path }) => (
                    <span
                        key={path}
                        className={`hover:text-gray-400 pb-0.5 cursor-pointer ${pathname?.includes(path) ? 'border-b-2 border-white' : ''}`}
                        onClick={() => handleNavigation(path)}
                    >
                        {label}
                    </span>
                ))}
            </div>
        </nav>
    );
}

export default TNavbar;
