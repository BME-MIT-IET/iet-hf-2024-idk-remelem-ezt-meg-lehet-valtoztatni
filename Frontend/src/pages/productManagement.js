import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Button from '@mui/material/Button';
import Grid from '@mui/material/Grid';
import { Input } from "@mui/material";

const baseUrl = process.env.REACT_APP_BACKEND_BASE_URL;

const ProductManagement = () => {
	const [files, setFiles] = useState([]);
	const navigate = useNavigate();

	const submit = () => {
		if (files.length < 1) {
			return;
		}
		var formdata = new FormData();
		formdata.append("file", files[0]);

		fetch(`${baseUrl}/api/Product/json`, {
			method: 'POST',
			body: formdata,
			credentials: "include"
		})
			.catch(err => {
				console.error(err);
				navigate("/error", { state: { errorMessage: err.message } });
			});
	}

	const setFile = (e) => {
		setFiles(e.target.files);
	}

	return (
		<>
			<Grid container spacing={2} columns={16}>
				<Grid item xs={8}>
					<Input onChange={setFile} type="file"></Input>
					<Button onClick={submit}>Upload</Button>
				</Grid>
			</Grid>
		</>
	);
}

export default ProductManagement;