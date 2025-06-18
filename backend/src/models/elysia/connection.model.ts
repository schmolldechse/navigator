import { t } from "elysia";

const LineColorSchema = t.Object(
	{
		lineName: t.String({ description: "The name of the line." }),
		hafasLineId: t.String({ description: "The ID of the line in the HAFAS system." }),
		hafasOperatorCode: t.String({ description: "The operator code in the HAFAS system." }),
		backgroundColor: t.String({ description: "The background color of the line." }),
		textColor: t.String({ description: "The text color of the line." }),
		borderColor: t.String({ description: "The border color of the line." }),
		shape: t.String({ description: "The shape of the line." })
	},
	{
		description:
			"LineColor represents the color scheme for a specific line in the transportation system. Data obtained from Traewelling"
	}
);

export { LineColorSchema };