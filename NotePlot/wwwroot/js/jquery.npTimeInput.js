/*
	Простой плагин для редактирования времени 
	в формате HH:mm
*/
(function( $ ) {
	var defaults = {
      sep_pos : 2
	  //np_event_mode: 0 // старый режим обработки клавиш - не Используя Event.key
	};		
	
	$.fn.npTimeInput = function() {				
	// функционал плагина
	// действия для всех DOМ-объектов jQuery + возращение объекта для поддержки цепочки методов	
		return this.each(function(){						
		
			$(this).attr('autocomplete','off'); // всегда off		 
				
			$(this).bind('paste' , function(e){ // блокируем paste
					e.preventDefault();
			});
			
			//KEYPRESS
			$(this).on('keypress.noteplot', function(event) {
				var 
					input = this,
                    pos =np_ti_getCaretPos(input),
                    keyCode = np_ti_getKeyCode(event);
				//FF пропускает несимволы, передаем по-умолчанию
				if (keyCode == 9 || keyCode == 27 || ((keyCode == 65 || keyCode == 97) && event.ctrlKey === true) || (keyCode >= 35 && keyCode <= 39)
				|| (keyCode >= 112 && keyCode <= 123 )  // для FF
				)
					return;						
				event.preventDefault();									
				//FF пропускает несимволы
				if ((keyCode < 48 || keyCode > 57) && (keyCode <= 96 || keyCode > 105 ) 
				)
					return;
				
				if (pos == defaults.sep_pos){			
					pos = pos+1;
				}
				np_ti_insertChar(input,pos,keyCode);
                pos =np_ti_getCaretPos(input);
				if (pos == defaults.sep_pos)
					np_ti_setCaretPos(input,pos+1);
			});
			
			//KEYDOWN
			$(this).on('keydown.noteplot', function(event) {
				var 
					val = $(this).val();	
					input = this,
                        pos = np_ti_getCaretPos(this),
                        keyCode = np_ti_getKeyCode(event);
										
				// del = 46 backspace = 8									
				// при удалении либо зануляем выделенные символы, либо зануляем символ
				if  ( keyCode == 46 || keyCode == 8){ 
					event.preventDefault();							
					//var sel = /*''+ */getSelectedText(); // [!!!]
                    var sel =  np_ti_getSelectedText(input);
					if (sel.length > 0)
					{
						var array = val.split('');					
						var ln = pos+sel.length;						
						for(var i = pos; i<ln; i++){						
							if (i != defaults.sep_pos)
								array[i] = '0';				
						}							
						$(this).val(array.join(''));
						np_ti_setCaretPos(input, pos);
					}
					else{
						np_ti_insertChar(input,pos,keyCode);
					}
				  return;						
				}
				// стрелка влево(37) и вправо(39)	
				if  ( keyCode == 37 || keyCode == 39){
					if (keyCode == 37 && !event.shiftKey){ // <--
						if (pos == defaults.sep_pos+1) {
							event.preventDefault();							
							pos = pos - 2;
							np_ti_setCaretPos(input, pos);
							return;
						}
					}	
					if (keyCode == 39 && !event.shiftKey){ // -->
						if (pos == defaults.sep_pos-1) {
							event.preventDefault();
							pos = pos + 2;
                            np_ti_setCaretPos(input, pos);
							return;
						}
					}					
				}
			
				// Разрешаем: backspace, delete, tab и escape
				if  ( keyCode == 46 || keyCode == 8 || keyCode == 9 || keyCode == 27 ||
					// Разрешаем: Ctrl+A
					(keyCode == 65 && event.ctrlKey === true) ||
					// Разрешаем: home, end, влево, вправо
					(keyCode >= 35 && keyCode <= 39) ||
					(keyCode >= 48 && keyCode <= 57) || (keyCode >= 96 && keyCode <= 105 ) ||
					//F1-F12
					(keyCode >= 112 && keyCode <= 123 )) 						
					{
					// Ничего не делаем
					return;
					}
				else
					event.preventDefault();
			});					
		});
	}
	
	// ОБЩИЕ ФУНКЦИИ
	//вернуть позицию курсора в поле ввода	
	// input - HTML element
    function np_ti_getCaretPos(input)
	{			  	
	  input.focus();
	  if(input.selectionStart) 
		return input.selectionStart;//Gecko
	  else 
	  if (document.selection)//IE
	  {
		var sel = document.selection.createRange();
		var clone = sel.duplicate();
		sel.collapse(true);
		clone.moveToElementText(input);
		clone.setEndPoint('EndToEnd', sel);
		return clone.text.length;
	  }	  
	  return 0;
	}										
		
	// выводим символ		
    function np_ti_insertChar(input,pos,keycode) {
		var 
			val = $(input).val(),
			key = keycode,
			array = val.split(''),
			chr='0';			
		if (keycode == 8 || keycode == 46)
			key = 48;
		if 	(keycode == 8 ){ // backspace
			 pos = pos - 1;
			 if (pos == defaults.sep_pos)
			 pos = pos - 1;
			 //key = 48;
			 np_ti_setCaretPos (input, pos);	
		}
		if 	(keycode == 46 ){ // del
			 //key = 48;
			 if (pos == defaults.sep_pos)
			 pos = pos + 1;			 
			 np_ti_setCaretPos (input, pos);				 
		}
		chr = String.fromCharCode(key);

		if (array[pos] && key !== 8 && key !== 46) {		  		
		  array.splice(pos, 1); 	 // удалили
		  array.splice(pos, 0, chr); // добавили		  
		  if ((array[0] > '2') || (array[3] > '5'))
			return;
		  if ((array[0] == '2') && (array[1] > '4')){
			if (pos == 1) return;
			array[1] = '0'
		  }				  
		  $(input).val(array.join(''));
		  if ((keycode != 8) && (keycode != 46)){
			np_ti_setCaretPos(input,pos+1); 
		  }
		  else
			np_ti_setCaretPos(input,pos);
		}
	}

	// установка позиции каретки
    function np_ti_setCaretPos (input, pos) {
		if (input.setSelectionRange) {
			input.setSelectionRange(pos, pos);
		} else if (input.createTextRange) {  // [!!!]
		var range = input.createTextRange();
		  range.collapse(true);
		  range.moveEnd('character', pos);
		  range.moveStart('character', pos);
		  range.select();
		}						
	}

/*		
		// возврат выделеннного текста на странице не работает в FF и IE https://bugzilla.mozilla.org/show_bug.cgi?id=85686
		//http://qaru.site/questions/26754/windowgetselection-of-textarea-not-working-in-firefox
		function getSelectedText(){
			var t = '';
			if(window.getSelection) {
				t = window.getSelection();//.toString(); //[!!!] toString()				
			} else if(document.getSelection) {
				t = document.getSelection();//.toString();
			} else if(document.selection && document.selection.createRange) {
				t = document.selection.createRange().text;
			}
			return t.toString();
		}
*/		
	
	// Этот вариант рабочий
    function np_ti_getSelectedText(input) {
		if (window.getSelection) { // Это работает для и для FF
			try {
				var ta = $(input).get(0);
				return ta.value.substring(ta.selectionStart, ta.selectionEnd);
			} catch (e) {
				console.log('Can not get selection text')
			}
		} 
		else if(document.getSelection) {
			return document.getSelection().toString();
		} 	
		// For IE
		else if (document.selection && document.selection.createRange && document.selection.type != "Control") {
			return document.selection.createRange().text;
		}
	}

    // хотя эти параметры события deprecated но работают лучше , чем event.key
	function np_ti_getKeyCode(event){
		var keyCode = (event.which || event.charCode || event.keyCode || 0);
		return keyCode;	
	}
	
})(jQuery);