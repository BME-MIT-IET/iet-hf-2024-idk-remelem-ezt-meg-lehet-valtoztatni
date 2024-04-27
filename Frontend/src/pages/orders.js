import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Button from '@mui/material/Button';

const baseUrl = process.env.REACT_APP_BACKEND_BASE_URL;

const Orders = () => {
	const [orders, setOrders] = useState([]);
	const [isAdmin, setIsAdmin] = useState(false);
	const navigate = useNavigate();

	const updateOrderStatus = (orderid, value) => {
		fetch(`${baseUrl}/api/Order/${orderid}`, {
			method: "PUT",
			headers: {
				"Content-Type": "application/json"
			},
			body: JSON.stringify({
				"OrderStatus": value
			}),
			credentials: "include"
		})
			.then(resp => {
				if (resp.status !== 204) {
					throw new Error();
				}
			})
			.then(updateOrder)
			.catch(err => {
				console.error(err);
				navigate("/error", { state: { errorMessage: err.message } });
			});
	}

	const updateOrder = () => {
		fetch(`${baseUrl}/api/Order`, {
			credentials: "include"
		})
			.then(resp => {
				if (resp.status !== 200) {
					throw new Error();
				}
				return resp.json()
			})
			.then(setOrders)
			.catch(err => {
				console.error(err);
				navigate("/error", { state: { errorMessage: err.message } });
			});
	}

	useEffect(updateOrder, []);

	useEffect(() => {
		setIsAdmin(localStorage.getItem("isAdmin") === "true");
	}, [localStorage.getItem("isAdmin")]);

	return (
		<>
			Megrendelések:
			<ol>
				{
					orders.map((order, index) => {
						return (
							<li key={index}>
								<p>Dátum:{order?.orderDate}</p>
								<p>Termékek:</p>
								<ul>
									{
										order.orderItems.map((item, index) =>
											<li key={index}>
												<p>{item?.product.name}</p>
												<p>{item?.product.price}</p>
												<p>{item?.product.description}</p>
											</li>
										)
									}
								</ul>
								<Button variant="contained" href={`${baseUrl}/api/Order/${order.id}/pdf`}>Számla</Button>
								<p>Státusz: {order?.orderStatus}</p>
								{
									isAdmin &&
									<>
										<Button variant="outlined" onClick={() => updateOrderStatus(order.id, "Unread")}>Unread</Button>
										<Button variant="outlined" onClick={() => updateOrderStatus(order.id, "Processing")}>Processing</Button>
										<Button variant="outlined" onClick={() => updateOrderStatus(order.id, "Shipping")}>Shipping</Button>
										<Button variant="outlined" onClick={() => updateOrderStatus(order.id, "Delivered")}>Deliver</Button>
										<Button variant="outlined" onClick={() => updateOrderStatus(order.id, "Rejected")}>Reject</Button>
									</>
								}
							</li>);
					})
				}
			</ol>
		</>
	);
}

export default Orders;