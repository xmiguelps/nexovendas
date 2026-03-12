CREATE DATABASE nexovendas;
USE nexovendas;
CREATE TABLE tb_cliente (
	cd_cliente INT AUTO_INCREMENT PRIMARY KEY,
    nm_cliente VARCHAR(100) NOT NULL,
    ds_email VARCHAR(100) NOT NULL
);
CREATE TABLE tb_produto (
	cd_produto INT PRIMARY KEY AUTO_INCREMENT,
    nm_produto VARCHAR(100) NOT NULL,
    vl_preco DECIMAL (10,2) NOT NULL
);
CREATE TABLE tb_venda (
	cd_venda INT AUTO_INCREMENT PRIMARY KEY,
    dt_venda DATETIME DEFAULT CURRENT_TIMESTAMP,
    fk_cliente INT,
    fk_produto INT,
    FOREIGN KEY (fk_cliente) REFERENCES tb_cliente(cd_cliente),
    FOREIGN KEY (fk_produto) REFERENCES tb_produto(cd_produto)
);