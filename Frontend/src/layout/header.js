import { Button, Grid, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

const baseUrl = process.env.REACT_APP_BASE_URL;

const Header = () => {
	const [username, setUsername] = useState("");
	const [isAdmin, setIsAdmin] = useState(false);
	const navigate = useNavigate();

	const logout = () => {
		fetch(`${baseUrl}/api/User/logout`, {
			method: "POST",
			credentials: "include"
		})
			.then(resp => {
				console.log(resp);
				localStorage.removeItem("id");
				localStorage.removeItem("name");
				localStorage.removeItem("isAdmin");
				navigate("/login");
			})
			.catch(console.error);
	}

	useEffect(() => {
		setUsername(localStorage.getItem("name") ?? "");
	}, [localStorage.getItem("name")]);

	useEffect(() => {
		setIsAdmin(localStorage.getItem("isAdmin") === "true");
	}, [localStorage.getItem("isAdmin")]);

	return (
		<nav>
			<div style={{ width: "100%", margin: "10px auto 20px auto" }}>
				<Grid
					container
					spacing={3}
					sx={{ flexGrow: 1 }}
					justifyContent="center"
					direction="row"
				>
					{
						username !== "" &&
						<Grid item>
							<Typography>{username}</Typography>
						</Grid>
					}

					<Grid item>
						<Button href="/" variant="contained">Főoldal</Button>
					</Grid>
					{
						username === "" ?
							<>
								<Grid item>
									<Button href="/login" variant="contained">Bejelentkezés</Button>
								</Grid>
								<Grid item>
									<Button href="/register" variant="contained">Regisztrálás</Button>
								</Grid>
							</>
							:
							<>
								<Grid item>
									<Button href="/product" variant="contained">Termékek</Button>
								</Grid>
								<Grid item>
									<Button href="/cart" variant="contained">Kosár</Button>
								</Grid>
								<Grid item>
									<Button href="/orders" variant="contained">Megrendelések</Button>
								</Grid>
								{
									isAdmin &&
									<Grid item>
										<Button href="/product_management" variant="contained">Termékek kezelése</Button>
									</Grid>
								}
								<Grid item>
									<Button onClick={logout} variant="contained">Kijelentkezés</Button>
								</Grid>
							</>
					}
				</Grid>
			</div>
		</nav>
	);
}


export default Header;