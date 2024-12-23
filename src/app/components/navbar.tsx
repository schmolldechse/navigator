'use client';

import {usePathname, useRouter} from "next/navigation";

interface NavbarProps {
    id: string
}

const Navbar: React.FC<NavbarProps> = ({id}) => {
    const router = useRouter();
    const pathname = usePathname();

    const navbarItems = [
        {label: "Departures", path: '/departures'},
        {label: "Arrivals", path: '/arrivals'},
        {label: "Statistics", path: '/statistics'},
    ]

    const handleNavigation = (path: string) => router.push(`/${id}${path}`);

    return (
        <nav className="text-2xl p-4 w-full">
            <div className="container mx-auto flex justify-between items-center cursor-pointer">
                <div
                    className="hover:text-gray-400 pb-2 border-white "
                    onClick={(e) => router.push("/")}
                >
                    Home
                </div>
                <div className="flex space-x-5">
                    {navbarItems.map(({label, path}) => (
                        <span
                            key={path}
                            className={`hover:text-gray-400 pb-2 ${pathname?.includes(path) ? 'border-b-2 border-white' : ''}`}
                            onClick={() => handleNavigation(path)}
                        >
                            {label}
                        </span>
                    ))}
                </div>
            </div>
        </nav>
    )
}

export default Navbar;
