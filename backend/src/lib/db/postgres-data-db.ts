import { drizzle } from "drizzle-orm/node-postgres";

export const db = drizzle(process.env.AUTH_POSTGRES_URL! + "?options=-csearch_path=data");
