import { drizzle } from "drizzle-orm/node-postgres";
import { Pool } from "pg";

const pool = new Pool({ connectionString: Bun.env.POSTGRES_CONNECTION_STRING, keepAlive: true });

const database = drizzle(pool);

export { pool, database };
