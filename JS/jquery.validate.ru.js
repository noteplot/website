// переопределение методов валидации на стороне клиента для десятичного разделителя - запятая 
//объявлять после валидатора:
//	<script src="../jquery.validate.js"></script>
//	<script src="../jquery.validate.ru.js"></script>

$.validator.methods.range = function (value, element, param) {
	var globalizedValue = value.replace(",", ".");
	return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
}
$.validator.methods.max = function (value, element, param) {
	var globalizedValue = value.replace(",", ".");
	return this.optional( element ) || globalizedValue <= param;
}
$.validator.methods.min = function (value, element, param) {
	var globalizedValue = value.replace(",", ".");
	return this.optional( element ) || globalizedValue >= param;
}
$.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
}