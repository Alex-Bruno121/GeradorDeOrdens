export const orderValidators = {
    validateQuantidade: (_: any, value: number) => {
        if (!Number.isInteger(value)) return Promise.reject('A quantidade deve ser um número inteiro');
        if (value <= 0) return Promise.reject('A quantidade deve ser positiva');
        if (value >= 100000) return Promise.reject('A quantidade deve ser menor que 100.000');
        return Promise.resolve();
    },

    validatePreco: (_: any, value: number) => {
        if (value <= 0) return Promise.reject('O preço deve ser positivo');
        if (value >= 1000) return Promise.reject('O preço deve ser menor que 1.000');

        const rounded = Math.round(value * 100);
        const original = value * 100;
        if (Math.abs(rounded - original) > 0.0000001)
            return Promise.reject('O preço deve ser múltiplo de 0.01');

        return Promise.resolve();
    }
};