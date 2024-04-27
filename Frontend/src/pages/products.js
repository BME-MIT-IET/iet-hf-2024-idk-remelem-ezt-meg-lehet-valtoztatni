import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Slider from '@mui/material/Slider';
import Typography from '@mui/material/Typography';
import Button from '@mui/material/Button';
import ButtonGroup from '@mui/material/ButtonGroup';
import Grid from '@mui/material/Grid';

const baseUrl = process.env.REACT_APP_BACKEND_BASE_URL;
const MAX_PRICE = 100_000;

const Product = () => {
	const queryParameters = new URLSearchParams(window.location.search);
	const categoryId = queryParameters.get("categoryId");
	const [products, setProducts] = useState([]);
	const [categories, setCategories] = useState([]);
	const [priceRange, setPriceRange] = useState({
		min: 0,
		max: MAX_PRICE,
	});
	const navigate = useNavigate();

	const addToCart = (item) => {
		const items = JSON.parse(localStorage.getItem("items")) || [];
		const itemIndex = items.findIndex((value) => value.id === item.id);
		if (itemIndex !== -1) {
			items[itemIndex] = { ...item, quantity: items[itemIndex].quantity + 1 };
		} else {
			items.push({ ...item, quantity: 1 });
		}
		localStorage.setItem("items", JSON.stringify(items));
	}

	const sliderChange = (event) => {
		const newValues = event.target.value;
		setPriceRange({
			min: newValues[0],
			max: newValues[1]
		});
	}

	const unnormalize = (num) => {
		return num * MAX_PRICE / 100;
	}

	useEffect(() => {
		const params = new URLSearchParams();
		params.append("minPrice", unnormalize(priceRange.min));
		params.append("maxPrice", unnormalize(priceRange.max));
		if (categoryId !== null) {
			params.append("categoryId", categoryId);
		}
		fetch(`${baseUrl}/api/Product?${params.toString()}`, {
			credentials: "include"
		})
			.then(resp => {
				if (resp.status !== 200) {
					throw new Error();
				}
				return resp.json()
			})
			.then(setProducts)
			.catch(err => {
				console.error(err);
				navigate("/error", { state: { errorMessage: err.message } });
			})
	}, [priceRange.min, priceRange.max, categoryId]);

	useEffect(() => {
		fetch(`${baseUrl}/api/Category`)
			.then(resp => {
				if (resp.status !== 200) {
					throw new Error();
				}
				return resp.json()
			})
			.then(setCategories)
			.catch(err => {
				console.error(err);
				navigate("/error", { state: { errorMessage: err.message } });
			});
	}, []);

	return (
		<>
			<Grid container spacing={2} columns={16}>
				<Grid item xs={8}>
					<div style={{ margin: "10px 100px", textAlign: "center" }}>
						<Typography>Filters:</Typography>
						<ButtonGroup
							orientation="vertical"
							aria-label="vertical outlined button group"
						>
							<Button href="?" variant={categoryId == null ? "contained" : "outlined"}>All</Button>
							{
								categories.map((value, index) => {
									return <Button variant={categoryId == value.id ? "contained" : "outlined"} href={`?categoryId=${value.id}`} key={index}>{value.name}</Button>;
								})
							}
						</ButtonGroup>
						<Typography id="non-linear-slider" gutterBottom>
							Price:
						</Typography>
						<Slider
							value={[priceRange.min, priceRange.max]}
							onChange={sliderChange}
							scale={unnormalize}
							valueLabelDisplay="auto"
							aria-labelledby="non-linear-slider"
						/>
					</div>
				</Grid>
				<Grid item xs={8}>
					<table style={{ margin: "auto" }}>
						<thead>
							<tr>
								<th>Name</th>
								<th>Description</th>
								<th>Price</th>
							</tr>
						</thead>
						<tbody>
							{
								products.map((value, index) => {
									return (
										<tr key={index}>
											<td>{value?.name}</td>
											<td>{value?.description}</td>
											<td>{value?.price}$</td>
											<td><Button variant="contained" onClick={() => addToCart(value)}>Add to cart</Button></td>
										</tr>);
								})
							}
						</tbody>
					</table>
				</Grid>
			</Grid>
		</>
	);
}

export default Product;