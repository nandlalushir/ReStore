import { Typography } from "@mui/material";
import { useEffect, useState } from "react";
import agent from "../../app/api/agent";
import { Basket } from "../../app/models/basket";
import LoadingComponent from "../../layout/LoadingComponent";

export default function BasketPage() {
  const [loading, setLoading] = useState(false);
  const [basket, setBasket] = useState<Basket | null>(null);

  useEffect(() => {
    agent.Basket.get()
      .then((basket) => setBasket(basket))
      .catch((error) => console.log(error))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <LoadingComponent message="Loading basket.." />;

  if (!basket) return <Typography variant="h3">Your basket is empty.</Typography>;

  return <h1>Buyer Id = {basket.buyerId}</h1>;
}
