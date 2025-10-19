import type { NavigationItem } from '$lib/models/navigation';
import type { LayoutServerLoad } from './$types';

export const load: LayoutServerLoad = async ({}) => {
	const pages: NavigationItem[] = [
		{ href: '/' },
		{ href: '/maps', label: 'Maps' },
		{ href: '/statistics', label: 'Statistics' }
	];

	return { pages };
};
