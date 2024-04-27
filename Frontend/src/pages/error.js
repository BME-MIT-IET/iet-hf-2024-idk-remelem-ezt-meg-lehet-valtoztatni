import { useLocation, useRouteError } from 'react-router-dom';

const getErrorDisplayMessage = errorMessage => {
	switch (errorMessage) {
		case "Failed to fetch":
			return "A szerver nem elérhető.";
		default:
			return "Váratlan hiba törént";
	}
}

const Error = () => {
	const error = useRouteError();
	console.log("ASD")
	console.log(error)

	const { state } = useLocation();
	console.log(state);
	const { errorMessage } = state || { errorMessage: "" };

	const errorDisplayMessage = getErrorDisplayMessage(errorMessage);

	return (
		<h1 style={{ textAlign: "center" }}>
			{errorDisplayMessage}
		</h1>
	);
}

export default Error;