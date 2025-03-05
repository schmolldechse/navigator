import { text, timestamp, boolean, pgSchema, pgEnum } from "drizzle-orm/pg-core";

export const roleEnum = pgEnum("role", ["default", "admin"]);

export const authSchema = pgSchema("auth");
export const user = authSchema.table("user", {
	id: text("id").notNull().primaryKey(),
	name: text("name").notNull(),
	email: text("email").notNull().unique(),
	emailVerified: boolean("emailVerified").notNull(),
	image: text("image"),
	createdAt: timestamp("createdAt", { precision: 3 }).notNull(),
	updatedAt: timestamp("updatedAt", { precision: 3 }).notNull(),
	username: text("username").notNull().unique(),
	role: roleEnum().notNull().default("default")
});

export const session = authSchema.table("session", {
	id: text("id").notNull().primaryKey(),
	expiresAt: timestamp("expiresAt", { precision: 3 }).notNull(),
	token: text("token").notNull().unique(),
	createdAt: timestamp("createdAt", { precision: 3 }).notNull(),
	updatedAt: timestamp("updatedAt", { precision: 3 }).notNull(),
	ipAddress: text("ipAddress"),
	userAgent: text("userAgent"),
	userId: text("userId")
		.notNull()
		.references(() => user.id)
});

export const account = authSchema.table("account", {
	id: text("id").notNull().primaryKey(),
	accountId: text("accountId").notNull(),
	providerId: text("providerId").notNull(),
	userId: text("userId")
		.notNull()
		.references(() => user.id),
	accessToken: text("accessToken"),
	refreshToken: text("refreshToken"),
	idToken: text("idToken"),
	accessTokenExpiresAt: timestamp("accessTokenExpiresAt", { precision: 3 }),
	refreshTokenExpiresAt: timestamp("refreshTokenExpiresAt", { precision: 3 }),
	scope: text("scope"),
	password: text("password"),
	createdAt: timestamp("createdAt", { precision: 3 }).notNull(),
	updatedAt: timestamp("updatedAt", { precision: 3 }).notNull()
});

export const verification = authSchema.table("verification", {
	id: text("id").notNull().primaryKey(),
	identifier: text("identifier").notNull(),
	value: text("value").notNull(),
	expiresAt: timestamp("expiresAt", { precision: 3 }).notNull(),
	createdAt: timestamp("createdAt", { precision: 3 }),
	updatedAt: timestamp("updatedAt", { precision: 3 })
});
