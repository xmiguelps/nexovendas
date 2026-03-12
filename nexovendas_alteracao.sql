USE nexovendas;

-- ALTER TABLE tb_venda DROP FOREIGN KEY tb_vendas_ibfk2;
-- ALTER TABLE tb_venda DROP COLUMN fk_produto;

CREATE TABLE tb_itens_venda (
	cd_item INT AUTO_INCREMENT PRIMARY KEY,
    fk_venda INT NOT NULL,
    fk_produto INT NOT NULL,
    qt_produto INT NOT NULL,
    vl_subtotal DECIMAL(10, 2) NOT NULL,
    FOREIGN KEY (fk_venda) REFERENCES tb_venda(cd_venda),
    FOREIGN KEY (fk_produto) REFERENCES tb_produto(cd_produto)
);