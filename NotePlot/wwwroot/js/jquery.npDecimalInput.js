/*
	Ф-ция npDecimalInput обеспечивает ввод чисел в формате decimal(p,s), аналогично ms sql 
	Разделитель десятичных знаков может быть или точка или запятая. Допускается ввод орицательных значений.	
	Все параметры формата: точность(precision), кол-во разрадов после запятой(scale), разделитель десятичных
	знаков и принак возможности ввода отрицательного числа задаются в атрибутах элемента ввода:
	np_dec_precision = "10" np_dec_scale = "2" np_dec_point='.' np_dec_minus
*/
(function ($) {
    // убираем лишние нули после разделителя
    $.fn.npDecimalFormat = function () {
        return this.each(function(){
            var
                val = $(this).val(),
                point = ($(this).attr('np_dec_point') && ($(this).attr('np_dec_point') == '.' || $(this).attr('np_dec_point') == ',')) ? $(this).attr('np_dec_point') : DecPointDefault(),                
                minus = $(this).get(0).hasAttribute('np_dec_minus') ? true : false,ixc, ix0, len;
                ixc = val.indexOf(point);
            
            while (true) {
                len = val.length;
                if (len == 0) break;
                ix0 = val.lastIndexOf('0');
                if ((ix0 >= 0) && (ixc >= 0) && (ix0 > ixc) && (ix0 == len - 1)) {
                    val = val.substr(0, ix0);
                }
                else {
                    if ((ixc > 0) && (ixc == len - 1)) {
                        val = val.substr(0, ixc);
                    }
                    break;
                }
            };            
            $(this).val(val);               
        });
    };

	$.fn.npDecimalInput = function() {
	// функционал плагина
	// действия для всех DOМ-объектов jQuery + возращение объекта для поддержки цепочки методов	
		return this.each(function(){						
			$(this).attr('autocomplete','off'); // всегда off		 
				
            $(this).bind('paste.noteplot' , function(e){ // блокируем paste
					e.preventDefault();
			});

			$(this).bind('keydown.noteplot' , function(e){
				var 
					val 		= $(this).val(),
					point 		= ($(this).attr('np_dec_point') && ($(this).attr('np_dec_point') == '.' || $(this).attr('np_dec_point') == ',')) ? $(this).attr('np_dec_point') : DecPointDefault(),
					scale 		= $(this).attr('np_dec_scale') ? parseInt($(this).attr('np_dec_scale')) : 0,
					minus 		= $(this).get(0).hasAttribute('np_dec_minus') ? true : false;							

					if (!CheckNumber(e, val, scale, point, minus))
					e.preventDefault();		
			});
				
			$(this).bind('keypress.noteplot' , function(e){
				var 
					val = $(this).val(),
					point 		= ($(this).attr('np_dec_point') && ($(this).attr('np_dec_point') == '.' || $(this).attr('np_dec_point') == ',')) ? $(this).attr('np_dec_point') : DecPointDefault(),
					precision 	= $(this).attr('np_dec_precision') ? parseInt($(this).attr('np_dec_precision')) : 1,
					minus 		= $(this).get(0).hasAttribute('np_dec_minus') ? true : false,			
					key = (e.keyCode ? e.keyCode : e.which);
					
                // здесь unicode - пропускаем только установленный разделитель
                // point minus comma  numbers    				
                if ( !CheckLength(val, point, precision, minus) || ( ( key != 44 && key != 46 && key != 45) && ( key < 48 || key > 57 ) ) )
                {
                    e.preventDefault();
                }                
			});

			$(this).bind('keyup.noteplot' , function(e){
				var 
					val 		= $(this).val(),
					point 		= ($(this).attr('np_dec_point') && ($(this).attr('np_dec_point') == '.' || $(this).attr('np_dec_point') == ',')) ? $(this).attr('np_dec_point') : DecPointDefault(),					//point		= DecPointDefault(),
					scale 		= $(this).attr('np_dec_scale') ? parseInt($(this).attr('np_dec_scale')) : 0,
					precision 	= $(this).attr('np_dec_precision') ? parseInt($(this).attr('np_dec_precision')) : 1,
					minus 		= $(this).get(0).hasAttribute('np_dec_minus') ? true : false,
					ix, len, s1, s2;
						
				// проверка на scale
				ix = val.indexOf(point),
				len = val.length;					
				
				if ( ix >=0 ) 
				{						
					if ( len - (ix+1) > scale ) // удаляем последний символ
					{
						$(this).val(val.substr(0, len-1));
					}						
				}				
				
				// проверка на minus - удаляем лишний минус
				ix = val.lastIndexOf('-');
				if ( minus && ix > 0)
				{	len = len+1
					s1 = val.substring(0, ix)	
					s2 = val.substring(ix+1, len-1)
					$(this).val(s1+s2);
				}

				// проверка на начальные нули
				ix = val.lastIndexOf('00');					
				if (ix == 0)
				{
					$(this).val(val.substr(0,1));
				}						
					
				// проверка на начальный нуль
                /*
				ix = val.lastIndexOf('0');					
				if (ix == 0 && (val.indexOf(point) == -1 || val.indexOf(point) > 0) && len > 1)
				{
					$(this).val(val.substring(0, ix)+val.substring(ix+1, val.len));
				}						
		        */
				// проверка на кол-во символов
				len = precision				
				if ((val.indexOf('-')>=0))
				(
					len = len +1
				)

				if ((val.indexOf(point)>=0) && (val.indexOf(point)!=len))
				{
					len = len +1
				}

				if (val.length > len)
				{
					$(this).val(val.substr(0, len));
				}							
			});

			
		});
	}	
	
	// текущий десятичный разделитель
	function DecPointDefault(){
		var num = new Number(0.1)			
		return num.toLocaleString().substr(1,1)
	};
		
	function CheckLength(val, point, precision, minus){
		var len = precision
		if (val.indexOf(point)>=0){
			len = len +1
		}
		
		if ((val.indexOf('-')>=0) || (minus == true))
		{
			len = len +1
		}
		
		if (val.length >= len)
		{
			return false
		}	
		else 
		{
			return true
		}
	};
		
	function CheckNumber(e,val, scale, point, minus){			
		var key = (e.keyCode ? e.keyCode : e.which);
		// ASCII codes
		if( // Control keys (delete, backspace, tab, enter, escape etc)
			key == 46 || key == 8 || key == 127 || key == 9 || key == 27 || key == 13 ||
			// Numbers
			(!e.shiftKey && key >= 48 && key <= 57 ) ||
			// point(190) or comma(188) ASCII
			(!e.shiftKey && key == 188 && scale >0 && val.indexOf(point) < 0 && point == ',') || // проверка количества символов + // десятичный разделитель уже введен? 
			(!e.shiftKey && key == 190 && scale >0 && val.indexOf(point) < 0 && point == '.') || 
			(!e.shiftKey && key == 189 && minus == true ) || // минус
			// Ctrl+A, Ctrl+R, Ctrl+P, Ctrl+S, Ctrl+F, Ctrl+H, Ctrl+B, Ctrl+J, Ctrl+T, Ctrl+Z, Ctrl++, Ctrl+-, Ctrl+0
			( (key == 65 || key == 82 || key == 80 || key == 83 || key == 70 || key == 72 || key == 66 || key == 74 || key == 84 || key == 90|| key == 61 || key == 173 || key == 48) && ( e.ctrlKey || e.metaKey ) === true ) ||
			// Ctrl+V, Ctrl+C, Ctrl+X				
			( (key == 86 || key == 67 || key == 88) && ( e.ctrlKey || e.metaKey ) === true ) ||
			// Ctrl+V - paste forbidden
			//( (key == 67 || key == 88) && ( e.ctrlKey || e.metaKey ) === true ) ||
			// home, end, left, right
			( (key >= 35 && key <= 39) ) ||
			// F1-F12 ???
			( (key >= 112 && key <= 123) )
		)
		{
			return true;
		}
		else
		{
			return false;
		}
	};
	
})(jQuery);