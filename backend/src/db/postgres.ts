import { drizzle } from "drizzle-orm/node-postgres";

const database = drizzle(process.env.POSTGRES_CONNECTION_STRING!);

export { database };