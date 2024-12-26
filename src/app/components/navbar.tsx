'use client';

import {Sheet, SheetContent, SheetHeader, SheetTitle, SheetTrigger} from "@/components/ui/sheet";
import {usePathname, useRouter} from "next/navigation";
import { Menu } from "lucide-react";

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
            <div className="container mx-auto flex justify-between items-center">
                {/* Home Link */}
                <div
                    className="hover:text-gray-400 pb-2 border-white cursor-pointer"
                    onClick={() => router.push("/")}
                >
                    Home
                </div>

                {/* Desktop Navigation */}
                <div className="hidden md:flex space-x-5">
                    {navbarItems.map(({ label, path }) => (
                        <span
                            key={path}
                            className={`hover:text-gray-400 pb-2 ${pathname?.includes(path) ? 'border-b-2 border-white' : ''}`}
                            onClick={() => handleNavigation(path)}
                        >
                            {label}
                        </span>
                    ))}
                </div>

                {/* Mobile Hamburger Menu */}
                <div className="md:hidden">
                    <Sheet>
                        <SheetTrigger>
                            <Menu className="w-8 h-8 cursor-pointer" />
                        </SheetTrigger>
                        <SheetContent className="w-[200px]">
                            <SheetHeader>
                                <SheetTitle></SheetTitle>
                            </SheetHeader>
                            <div className="mt-4 flex flex-col space-y-4">
                                {navbarItems.map(({ label, path }) => (
                                    <div
                                        key={path}
                                        className={`${pathname?.includes(path) ? 'font-bold' : ''}`}
                                        onClick={() => handleNavigation(path)}
                                    >
                                        {label}
                                    </div>
                                ))}
                            </div>
                        </SheetContent>
                    </Sheet>
                </div>
            </div>
        </nav>
    );
}

export default Navbar;
