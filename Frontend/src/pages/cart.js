import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Button from '@mui/material/Button';

const baseUrl = process.env.REACT_APP_BACKEND_BASE_URL;

const Cart = () => {
	const [items, setItems] = useState(JSON.parse(localStorage.getItem("items")) || []);
	const navigate = useNavigate();

	const removeItem = (itemIndex) => {
		const newItems = items.filter((_, index) => index !== itemIndex);
		setItems(newItems);
		localStorage.setItem("items", JSON.stringify(newItems));
	}

	const OrderItems = () => {
		fetch(`${baseUrl}/api/Order`, {
			method: "POST",
			headers: {
				"Content-Type": "application/json"
			},
			body: JSON.stringify({
				orderItems: items.map(item => ({ productId: item.id, quantity: item.quantity }))
			}),
			credentials: "include"
		})
			.then(resp => {
				if (resp.status !== 200) {
					throw new Error();
				}
				return resp.json()
			})
			.then(() => {
				localStorage.removeItem("items");
				navigate("/orders")
			})
			.catch(err => {
				console.error(err);
				navigate("/error", { state: { errorMessage: err.message } });
			});
	}

	return (
		<>
			<table style={{ margin: "auto" }}>
				<thead>
					<tr>
						<th>Name</th>
						<th>Description</th>
						<th>Price</th>
						<th>Quantity</th>
					</tr>
				</thead>
				<tbody>
					{
						items.map((value, index) => {
							return (
								<tr key={index}>
									<td>{value?.name}</td>
									<td>{value?.description}</td>
									<td>{value?.price}$</td>
									<td>{value?.quantity}</td>
									<td><Button variant="contained" onClick={() => removeItem(index)}>Remove</Button></td>
								</tr>);
						})
					}
				</tbody>
			</table>
			<Button onClick={OrderItems}>Megrendel</Button>
		</>
	);
}

export default Cart;