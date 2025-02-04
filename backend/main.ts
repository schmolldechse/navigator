import { Application, Router } from "@oak/oak";

const app = new Application();
const port = 8000;

const router = new Router();
router.post("/api/v1/stations", async (ctx) => {
  ctx.response.body = "Hello World";
});

app.use(router.routes());
app.use(router.allowedMethods());

app.addEventListener("listen", () => console.log(`Server running on port ${port}`));
await app.listen({ port });
