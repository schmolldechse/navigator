import type { BetterAuthPlugin, Session, User } from "better-auth";
import { APIError, createAuthEndpoint, createAuthMiddleware, getSessionFromCtx } from "better-auth/api";
import { z } from "zod";

interface UserWithRole extends User {
	role?: string;
}

export const rolePlugin = () => {
	const adminMiddleware = createAuthMiddleware(async (ctx) => {
		const session = await getSessionFromCtx(ctx);
		if (!session?.session || session?.user?.role !== "admin") throw new APIError("UNAUTHORIZED");

		return { session } as {
			session: {
				user: UserWithRole;
				session: Session;
			};
		};
	});

	return {
		id: "role-plugin",
		schema: {
			user: {
				fields: {
					role: {
						type: "string",
						fieldName: "role",
						required: true
					}
				}
			}
		},
		init(ctx) {
			return {
				options: {
					databaseHooks: {
						user: {
							create: {
								async before(user) {
									return {
										data: {
											...user,
											role: "default"
										}
									};
								}
							}
						}
					}
				}
			};
		},
		endpoints: {
			setRole: createAuthEndpoint(
				"/admin/set-role",
				{
					method: "POST",
					body: z.object({
						userId: z.string({
							description: "The user ID to set the role for"
						}),
						role: z.string({
							description: "The role to set. `admin` or `default`"
						})
					}),
					use: [adminMiddleware]
				},
				async (ctx) => {
					const updatedUser = await ctx.context.internalAdapter.updateUser(
						ctx.body.userId,
						{
							role: ctx.body.role
						},
						ctx
					);
					if (!updatedUser) throw new APIError("BAD_REQUEST");
					return ctx.json({ user: updatedUser as UserWithRole });
				}
			)
		}
	} satisfies BetterAuthPlugin;
};
