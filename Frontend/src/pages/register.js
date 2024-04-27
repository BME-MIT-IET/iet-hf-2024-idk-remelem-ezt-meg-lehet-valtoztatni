import { Button, Stack, TextField } from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

const baseUrl = process.env.REACT_APP_BACKEND_BASE_URL;

const Register = () => {
	const [name, setName] = useState("");
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const navigate = useNavigate();

	const changeName = (event) => {
		const newValue = event.target.value;
		setName(newValue);
	}

	const changeEmail = (event) => {
		const newValue = event.target.value;
		setEmail(newValue);
	}

	const changePassword = (event) => {
		const newValue = event.target.value;
		setPassword(newValue);
	}

	const register = () => {
		fetch(`${baseUrl}/api/User/register`, {
			method: "POST",
			cors: "no-cors",
			headers: {
				"Content-Type": "application/json"
			},
			body: JSON.stringify({ name, email, password })
		})
			.then(resp => {
				if (resp.status !== 201) {
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
					label="Name"
					defaultValue=""
					value={name}
					onChange={changeName}
				/>
				<TextField
					required
					id="outlined-required"
					label="Email"
					defaultValue=""
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
				<Button onClick={register}>Register</Button>
			</Stack>
		</div>
	);
}

export default Register;