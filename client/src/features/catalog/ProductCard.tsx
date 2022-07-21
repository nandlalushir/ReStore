import { Avatar, Button, Card, CardActions, CardContent, CardHeader, CardMedia, ListItem, ListItemAvatar, ListItemText, Typography } from "@mui/material";
import { useState } from "react";
import { Link } from "react-router-dom";
import agent from "../../app/api/agent";
import { Product } from "../../app/models/product";

interface Props {
  product: Product;
}

export default function ProductCard({ product }: Props) {
  const [loading, setLoading] = useState(false);

  function handleAddItem(productId: number) {
    setLoading(true);
    agent.Basket.addItem(productId)
      .catch((error) => console.log(error))
      .finally(() => setLoading(false));
  }

  return (
    <>
      <Card>
        <CardHeader avatar={<Avatar sx={{ bgcolor: "secondary.main" }}>{product.name.charAt(0).toUpperCase()}</Avatar>} title={product.name} titleTypographyProps={{ sx: { fontWeight: "bold", color: "primary.main" } }} />
        <CardMedia sx={{ height: 140, backgroundSize: "contain" }} image={product.pictureUrl} title={product.name} />
        <CardContent>
          <Typography gutterBottom variant="h5" color="text.secondary">
            ${(product.price / 100).toFixed(2)}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {product.brand} / {product.type}
          </Typography>
        </CardContent>
        <CardActions>
          <Button size="small" onClick={() => handleAddItem(product.id)}>
            Add to cart
          </Button>
          <Button component={Link} to={`/catalog/${product.id}`} size="small">
            View
          </Button>
        </CardActions>
      </Card>
    </>
  );
}
