import { Button, Stack, TextField } from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

const baseUrl = process.env.REACT_APP_BACKEND_BASE_URL;

const Login = () => {
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const navigate = useNavigate();

	const changeEmail = (event) => {
		const newValue = event.target.value;
		setEmail(newValue);
	}

	const changePassword = (event) => {
		const newValue = event.target.value;
		setPassword(newValue);
	}

	const login = () => {
		fetch(`${baseUrl}/api/User/login`, {
			method: "POST",
			headers: {
				"Content-Type": "application/json"
			},
			credentials: "include",
			body: JSON.stringify({ email, password })
		})
			.then(resp => {
				if (resp.status !== 200) {
					throw new Error();
				}
				return resp.json()
			})
			.then(user => {
				localStorage.setItem("id", user.id);
				localStorage.setItem("name", user.name);
				localStorage.setItem("isAdmin", user.isAdmin);
				navigate("/");
			})
			.catch(err => {
				console.error(err);
				navigate("/error", { state: { errorMessage: err.message } });
			});
	}

	return (
		<div >
			<Stack width={250} style={{ margin: "auto" }} spacing={2}>
				<TextField
					required
					id="outlined-required"
					label="Email"
					value={email}
					onChange={changeEmail}
				/>
				<TextField
					required
					id="outlined-password-input"
					label="Password"
					type="password"
					autoComplete="current-password"
					value={password}
					onChange={changePassword}
				/>
				<Button onClick={login}>Login</Button>
			</Stack>
		</div>
	);
}

export default Login;