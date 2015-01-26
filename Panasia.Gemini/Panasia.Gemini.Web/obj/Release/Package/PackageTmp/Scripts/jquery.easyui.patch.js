/**
 * The Patch for jQuery EasyUI 1.3.6
 */
(function($){
	$.fn._fit = function(fit){
		fit = fit == undefined ? true : fit;
		var t = this[0];
		var p = (t.tagName == 'BODY' ? t : this.parent()[0]);
		var fcount = p.fcount || 0;
		if (fit){
			if (!t.fitted){
				t.fitted = true;
				p.fcount = fcount + 1;
				$(p).addClass('panel-noscroll');
				if (p.tagName == 'BODY'){
					$('html').addClass('panel-fit');
				}
			}
		} else {
			if (t.fitted){
				t.fitted = false;
				p.fcount = fcount - 1;
				if (p.fcount == 0){
					$(p).removeClass('panel-noscroll');
					if (p.tagName == 'BODY'){
						$('html').removeClass('panel-fit');
					}
				}
			}
		}
		return {
			width: $(p).width() || 1,
			height: $(p).height() || 1
		}
	}
})(jQuery);

(function($){
	var plugin = $.fn.linkbutton;
	$.fn.linkbutton = function(options, param){
		var result = plugin.call(this, options, param);
		if (typeof options != 'string'){
			this.each(function(){
				$(this).unbind('click.linkbutton').bind('click.linkbutton', function(){
					var opts = $(this).linkbutton('options');
					if (!opts.disabled){
						if (opts.toggle){
							if (opts.selected){
								$(this).linkbutton('unselect');
							} else {
								$(this).linkbutton('select');
							}
						}
						opts.onClick.call(this);
					}
				});
			});
		}
		return result;
	};
	$.fn.linkbutton.methods = plugin.methods;
	$.fn.linkbutton.parseOptions = plugin.parseOptions;
	$.fn.linkbutton.defaults = plugin.defaults;
})(jQuery);

(function($){
	function setMenuSize(target, menu){
		var opts = $.data(target, 'menu').options;
		var style = menu.attr('style') || '';
		menu.css({
			display: 'block',
			left:-10000,
			height: 'auto',
			overflow: 'hidden'
		});
		
		var el = menu[0];
		var width = el.originalWidth || 0;
		if (!width){
			width = 0;
			menu.find('div.menu-text').each(function(){
				if (width < $(this)._outerWidth()){
					width = $(this)._outerWidth();
				}
				$(this).closest('div.menu-item')._outerHeight($(this)._outerHeight()+2);
			});
			width += 40;
		}
		
		width = Math.max(width, opts.minWidth);
		var height = el.originalHeight || 0;
		if (!height){
			height = menu.outerHeight();
			
			if (menu.hasClass('menu-top') && opts.alignTo){
				var at = $(opts.alignTo);
				var h1 = at.offset().top - $(document).scrollTop();
				var h2 = $(window)._outerHeight() + $(document).scrollTop() - at.offset().top - at._outerHeight();
				height = Math.min(height, Math.max(h1, h2));
			} else if (height > $(window)._outerHeight()){
				height = $(window).height();
				style += ';overflow:auto';
			} else {
				style += ';overflow:hidden';
			}
		}
		var lineHeight = Math.max(el.originalHeight, menu.outerHeight()) - 2;
		menu._outerWidth(width)._outerHeight(height);
		menu.children('div.menu-line')._outerHeight(lineHeight);
		
		style += ';width:' + el.style.width + ';height:' + el.style.height;
		
		menu.attr('style', style);
	}
	function showMenu(target, param){
		var left,top;
		param = param || {};
		var menu = $(param.menu || target);
		$(target).menu('resize', menu[0]);
		if (menu.hasClass('menu-top')){
			var opts = $.data(target, 'menu').options;
			$.extend(opts, param);
			left = opts.left;
			top = opts.top;
			if (opts.alignTo){
				var at = $(opts.alignTo);
				left = at.offset().left;
				top = at.offset().top + at._outerHeight();
				if (opts.align == 'right'){
					left += at.outerWidth() - menu.outerWidth();
				}
			}
			if (left + menu.outerWidth() > $(window)._outerWidth() + $(document)._scrollLeft()){
				left = $(window)._outerWidth() + $(document).scrollLeft() - menu.outerWidth() - 5;
			}
			if (left < 0){left = 0;}
			top = _fixTop(top, opts.alignTo);
		} else {
			var parent = param.parent;	// the parent menu item
			left = parent.offset().left + parent.outerWidth() - 2;
			if (left + menu.outerWidth() + 5 > $(window)._outerWidth() + $(document).scrollLeft()){
				left = parent.offset().left - menu.outerWidth() + 2;
			}
			top = _fixTop(parent.offset().top - 3);
		}
		
		function _fixTop(top, alignTo){
			if (top + menu.outerHeight() > $(window)._outerHeight() + $(document).scrollTop()){
				if (alignTo){
					top = $(alignTo).offset().top - menu._outerHeight();
				} else {
					top = $(window)._outerHeight() + $(document).scrollTop() - menu.outerHeight();
				}
			}
			if (top < 0){top = 0;}
			return top;
		}
		
		menu.css({left:left,top:top});
		menu.show(0, function(){
			if (!menu[0].shadow){
				menu[0].shadow = $('<div class="menu-shadow"></div>').insertAfter(menu);
			}
			menu[0].shadow.css({
				display:'block',
				zIndex:$.fn.menu.defaults.zIndex++,
				left:menu.css('left'),
				top:menu.css('top'),
				width:menu.outerWidth(),
				height:menu.outerHeight()
			});
			menu.css('z-index', $.fn.menu.defaults.zIndex++);
			if (menu.hasClass('menu-top')){
				$.data(menu[0], 'menu').options.onShow.call(menu[0]);
			}
		});
	}
	
	function setVisible(target, itemEl, visible){
		var menu = $(itemEl).parent();
		if (visible){
			$(itemEl).show();
		} else {
			$(itemEl).hide();
		}
		setMenuSize(target, menu);
	}
	
	$.extend($.fn.menu.methods, {
		show: function(jq, pos){
			return jq.each(function(){
				showMenu(this, pos);
			});
		},
		showItem: function(jq, itemEl){
			return jq.each(function(){
				setVisible(this, itemEl, true);
			});
		},
		hideItem: function(jq, itemEl){
			return jq.each(function(){
				setVisible(this, itemEl, false);
			});
		},
		resize: function(jq, menuEl){
			return jq.each(function(){
				setMenuSize(this, $(menuEl));
			});
		}
	});
	
	var removeMenuItem = $.fn.menu.methods.removeItem;
	$.fn.menu.methods.removeItem = function(jq, itemEl){
		return jq.each(function(){
			var menu = $(itemEl).parent();
			removeMenuItem.call($.fn.menu.methods, $(this), itemEl);
			setMenuSize(this, menu);
		});
	}

})(jQuery);

(function($){
	var plugin = $.fn.menubutton;
	$.fn.menubutton = function(options, param){
		var result = plugin.call(this, options, param);
		if (typeof options != 'string'){
			this.each(function(){
				var target = this;
				var opts = $.data(target, 'menubutton').options;
				var btn = $(target);
				var t = btn.find('.'+opts.cls.trigger);
				if (!t.length){t = btn}
				t.bind('mouseleave', function(){
					$(opts.menu).triggerHandler('mouseleave');
				});
			});
		}
		return result;
	};
	$.fn.menubutton.methods = plugin.methods;
	$.fn.menubutton.parseOptions = plugin.parseOptions;
	$.fn.menubutton.defaults = plugin.defaults;
	
})(jQuery);

(function($){
	$.fn.datagrid.defaults.loader = function(param, success, error){
		var target = this;
		var opts = $(target).datagrid('options');
		if (!opts.url) return false;
		$.ajax({
			type: opts.method,
			url: opts.url,
			data: param,
			dataType: 'json',
			success: function(data){
				var onLoadSuccess = opts.onLoadSuccess;
				opts.onLoadSuccess = function(){};
				var dc = $(target).data('datagrid').dc;
				dc.header1.add(dc.header2).find('input[type=checkbox]')._propAttr('checked', false);
				success(data);
				opts.onLoadSuccess = onLoadSuccess;
				opts.onLoadSuccess.call(target, data);
			},
			error: function(){
				error.apply(this, arguments);
			}
		});
	};
	
	var loadData = $.fn.datagrid.methods.loadData;
	$.fn.datagrid.methods.loadData = function(jq, data){
		return jq.each(function(){
			var dc = $(this).data('datagrid').dc;
			dc.header1.add(dc.header2).find('input[type=checkbox]')._propAttr('checked', false);
			loadData.call($.fn.datagrid.methods, $(this), data);
		});
	};
	
	var load = $.fn.datagrid.methods.load;
	$.fn.datagrid.methods.load = function(jq, param){
		return jq.each(function(){
			if (typeof param == 'string'){
				$(this).datagrid('options').url = param;
				param = null;
			}
			load.call($.fn.datagrid.methods, $(this), param);
		});
	};
	
	var reload = $.fn.datagrid.methods.reload;
	$.fn.datagrid.methods.reload = function(jq, param){
		return jq.each(function(){
			if (typeof param == 'string'){
				$(this).datagrid('options').url = param;
				param = null;
			}
			reload.call($.fn.datagrid.methods, $(this), param);
		});
	};
})(jQuery);

(function($){
	$.fn.treegrid.defaults.view.updateRow = function(target, id, row){
		var state = $.data(target, 'treegrid');
		var opts = state.options;
		var rowData = $(target).treegrid('find', id);
		$.extend(rowData, row);
		var depth = $(target).treegrid('getLevel', id) - 1;
		var styleValue = opts.rowStyler ? opts.rowStyler.call(target, rowData) : '';
		var newId = rowData[opts.idField];
		var rowIdPrefix = $.data(target, 'datagrid').rowIdPrefix;
		
		function _update(frozen){
			var fields = $(target).treegrid('getColumnFields', frozen);
			var tr = opts.finder.getTr(target, id, 'body', (frozen?1:2));
			var rownumber = tr.find('div.datagrid-cell-rownumber').html();
			var checked = tr.find('div.datagrid-cell-check input[type=checkbox]').is(':checked');
			tr.html(this.renderRow(target, fields, frozen, depth, rowData));
			tr.attr('style', styleValue || '');
			tr.find('div.datagrid-cell-rownumber').html(rownumber);
			if (checked){
				tr.find('div.datagrid-cell-check input[type=checkbox]')._propAttr('checked', true);
			}
			if (newId != id){
				tr.attr('id', rowIdPrefix + '-' + (frozen?1:2) + '-' + newId);
				tr.attr('node-id', newId);
			}
		}
		
		_update.call(this, true);
		_update.call(this, false);
		$(target).treegrid('fixRowHeight', id);
	}
	$.extend($.fn.treegrid.defaults.view, {
		render: function(target, container, frozen){
			var opts = $.data(target, 'treegrid').options;
			var fields = $(target).datagrid('getColumnFields', frozen);
			var rowIdPrefix = $.data(target, 'datagrid').rowIdPrefix;
			
			if (frozen){
				if (!(opts.rownumbers || (opts.frozenColumns && opts.frozenColumns.length))){
					return;
				}
			}
			
			var view = this;
			if (this.treeNodes && this.treeNodes.length){
				var table = getTreeData(frozen, this.treeLevel, this.treeNodes);
				$(container).append(table.join(''));
			}
			
			function getTreeData(frozen, depth, children){
				var pnode = $(target).treegrid('getParent', children[0][opts.idField]);
				var index = (pnode ? pnode.children.length : $(target).treegrid('getRoots').length) - children.length;
				
				var table = ['<table class="datagrid-btable" cellspacing="0" cellpadding="0" border="0"><tbody>'];
				for(var i=0; i<children.length; i++){
					var row = children[i];
					if (row.state != 'open' && row.state != 'closed'){
						row.state = 'open';
					}
					var css = opts.rowStyler ? opts.rowStyler.call(target, row) : '';
					var classValue = '';
					var styleValue = '';
					if (typeof css == 'string'){
						styleValue = css;
					} else if (css){
						classValue = css['class'] || '';
						styleValue = css['style'] || '';
					}
					var cls = 'class="datagrid-row ' + (index++ % 2 && opts.striped ? 'datagrid-row-alt ' : ' ') + classValue + '"';
					var style = styleValue ? 'style="' + styleValue + '"' : '';
					
					var rowId = rowIdPrefix + '-' + (frozen?1:2) + '-' + row[opts.idField];
					table.push('<tr id="' + rowId + '" node-id="' + row[opts.idField] + '" ' + cls + ' ' + style + '>');
					table = table.concat(view.renderRow.call(view, target, fields, frozen, depth, row));
					table.push('</tr>');
					
					if (row.children && row.children.length){
						var tt = getTreeData(frozen, depth+1, row.children);
						var v = row.state == 'closed' ? 'none' : 'block';
						
						table.push('<tr class="treegrid-tr-tree"><td style="border:0px" colspan=' + (fields.length + (opts.rownumbers?1:0)) + '><div style="display:' + v + '">');
						table = table.concat(tt);
						table.push('</div></td></tr>');
					}
				}
				table.push('</tbody></table>');
				return table;
			}
		}
	});
})(jQuery);

(function($){
	var currTarget;
	function stopEditing(target){
		var t = $(target);
		if (!t.length){return}
		var opts = $.data(target, 'propertygrid').options;
		var index = opts.editIndex;
		if (index == undefined){return;}
		var ed = t.datagrid('getEditors', index)[0];
		if (ed){
			ed.target.blur();
			if (t.datagrid('validateRow', index)){
				t.datagrid('endEdit', index);
			} else {
				t.datagrid('cancelEdit', index);
			}
		}
		opts.editIndex = undefined;
	}
	
	var plugin = $.fn.propertygrid;
	$.fn.propertygrid = function(options, param){
		var result = plugin.call(this, options, param);
		if (typeof options != 'string'){
			this.each(function(){
				var opts = $(this).propertygrid('options');
				$(this).datagrid($.extend({}, opts, {
					view:(opts.showGroup ? opts.groupView : opts.view),
					onClickRow:function(index,row){
						if (currTarget != this){
							stopEditing(currTarget);
							currTarget = this;
						}
						if (opts.editIndex != index && row.editor){
							var col = $(this).datagrid('getColumnOption', "value");
							col.editor = row.editor;
							stopEditing(currTarget);
							$(this).datagrid('beginEdit', index);
							var editors = $(this).datagrid('getEditors', index);
							if (editors.length){
								editors[0].target.focus();
								opts.editIndex = index;
							}
						}
						opts.onClickRow.call(this, index, row);
					},
					loadFilter:function(data){
						stopEditing(this);
						return opts.loadFilter.call(this, data);
					}
				}));
				$(document).unbind('.propertygrid').bind('mousedown.propertygrid', function(e){
					var p = $(e.target).closest('div.datagrid-view,div.combo-panel');
					if (p.length){return;}
					stopEditing(currTarget);
					currTarget = undefined;
				});
			});
		}
		return result;
	};
	$.fn.propertygrid.methods = plugin.methods;
	$.fn.propertygrid.defaults = plugin.defaults;
	$.fn.propertygrid.parseOptions = plugin.parseOptions;
})(jQuery);

(function($){
	var expandPanel = $.fn.panel.methods.expand;
	$.fn.panel.methods.expand = function(jq, animate){
		return jq.each(function(){
			var state = $(this).data('panel');
			var opts = $(this).panel('options');
			var onBeforeCollapse = opts.onBeforeCollapse;
			opts.onBeforeCollapse = function(){
				if (onBeforeCollapse.call(this) == false){return false;}
				if ($('body>div.combo-p>div.combo-panel:visible').length){
					return false;
				}
			}
			if (!opts.collapsed){return;}
			$(this).stop(true,true);
			if (!state.isLoaded && opts.content){
				state.isLoaded = true;
				$(this).html(opts.content);
				$.parser.parse($(this));
			}
			expandPanel.call($.fn.panel.methods, $(this), animate);
		});
	};
})(jQuery);

(function($){
	var plugin = $.fn.window;
	$.fn.window = function(options, param){
		var result = plugin.call(this, options, param);
		if (typeof options != 'string'){
			this.each(function(){
				var state = $.data(this, 'window');
				state.window.resizable({
					onStartResize:function(e){
						if (state.pmask){state.pmask.remove();}
						state.pmask = $('<div class="window-proxy-mask"></div>').insertAfter(state.window);
						state.pmask.css({
							zIndex: $.fn.window.defaults.zIndex++,
							left: e.data.left,
							top: e.data.top,
							width: state.window._outerWidth(),
							height: state.window._outerHeight()
						});
						if (state.proxy){state.proxy.remove();}
						state.proxy = $('<div class="window-proxy"></div>').insertAfter(state.window);
						state.proxy.css({
							zIndex: $.fn.window.defaults.zIndex++,
							left: e.data.left,
							top: e.data.top
						});
						state.proxy._outerWidth(e.data.width)._outerHeight(e.data.height);
					}
				});
			});
		}
		return result;
	};
	$.fn.window.methods = plugin.methods;
	$.fn.window.parseOptions = plugin.parseOptions;
	$.fn.window.defaults = plugin.defaults;
})(jQuery);

(function($){
	function buildDialog(target){
		var opts = $.data(target, 'dialog').options;
		
		if (opts.toolbar){
			if ($.isArray(opts.toolbar)){
				$(target).siblings('div.dialog-toolbar').remove();
				var toolbar = $('<div class="dialog-toolbar"><table cellspacing="0" cellpadding="0"><tr></tr></table></div>').appendTo(target);
				var tr = toolbar.find('tr');
				for(var i=0; i<opts.toolbar.length; i++){
					var btn = opts.toolbar[i];
					if (btn == '-'){
						$('<td><div class="dialog-tool-separator"></div></td>').appendTo(tr);
					} else {
						var td = $('<td></td>').appendTo(tr);
						var tool = $('<a href="javascript:void(0)"></a>').appendTo(td);
						tool[0].onclick = eval(btn.handler || function(){});
						tool.linkbutton($.extend({}, btn, {
							plain:true
						}));
					}
				}
			} else {
				$(opts.toolbar).addClass('dialog-toolbar').appendTo(target);
				$(opts.toolbar).show();
			}
		} else {
			$(target).siblings('div.dialog-toolbar').remove();
		}
		
		if (opts.buttons){
			if ($.isArray(opts.buttons)){
				$(target).siblings('div.dialog-button').remove();
				var buttons = $('<div class="dialog-button"></div>').appendTo(target);
				for(var i=0; i<opts.buttons.length; i++){
					var p = opts.buttons[i];
					var button = $('<a href="javascript:void(0)"></a>').appendTo(buttons);
					if (p.handler) button[0].onclick = p.handler;
					button.linkbutton(p);
				}
			} else {
				$(opts.buttons).addClass('dialog-button').appendTo(target);
				$(opts.buttons).show();
			}
		} else {
			$(target).siblings('div.dialog-button').remove();
		}
		
		var tb = $(target).children('.dialog-toolbar');
		var bb = $(target).children('.dialog-button');
		
		$(target).css({
			marginTop:(tb._outerHeight()-tb.length)+'px',
			marginBottom:(bb._outerHeight()-bb.length)+'px'
		});
		
		var spacer = $('<div class="dialog-spacer"></div>').prependTo(target);
		$(target).window($.extend({}, opts, {
			onResize: function(w, h){
				setContentSize(target);
				var s = $(this).children('div.dialog-spacer');
				if (s.length){
					setTimeout(function(){
						s.remove();
					},0);
				}
				opts.onResize.call(this, w, h);
			}
		}));
	}
	
	function setContentSize(target, param){
		var t = $(target);
		t.children('.dialog-toolbar,.dialog-button').css('position','absolute').appendTo(t.parent());
		
		var tb = t.siblings('.dialog-toolbar');
		var bb = t.siblings('.dialog-button');
		
		t._outerHeight(t._outerHeight()-tb._outerHeight()-bb._outerHeight()+tb.length+bb.length);
		
		var bcolor = t.css('borderColor');
		tb.css({
			top: (t.position().top-1+parseInt(t.css('borderTopWidth')))+'px',
			borderLeft:'1px solid '+bcolor,
			borderRight:'1px solid '+bcolor,
			borderTop:'1px solid '+bcolor
		});
		bb.css({
			top: t.position().top + t.outerHeight(true)-bb._outerHeight(),
			borderLeft:'1px solid '+bcolor,
			borderRight:'1px solid '+bcolor,
			borderBottom:'1px solid '+bcolor
		})
		tb.add(bb)._outerWidth(t._outerWidth());
		
		var shadow = $.data(target, 'window').shadow;
		if (shadow){
			var cc = t.panel('panel');
			shadow.css({
				width: cc._outerWidth(),
				height: cc._outerHeight()
			});
		}
	}
	
	var plugin = $.fn.dialog;
	$.fn.dialog = function(options, param){
		if (typeof options == 'string'){
			return plugin.call(this, options, param);
		} else {
			options = options || {};
			return this.each(function(){
				var state = $.data(this, 'dialog');
				if (state){
					$.extend(state.options, options);
				} else {
					state = $.data(this, 'dialog', {
						options: $.extend({}, $.fn.dialog.defaults, $.fn.dialog.parseOptions(this), options)
					});
				}
				buildDialog(this);
			});
		}
	};
	$.fn.dialog.methods = plugin.methods;
	$.fn.dialog.parseOptions = plugin.parseOptions;
	$.fn.dialog.defaults = plugin.defaults;
})(jQuery);

(function($){
	$.extend($.fn.tree.methods, {
		getRoot: function(jq, nodeEl){
			if (nodeEl){
				var target = nodeEl;
				var p = jq.tree('getParent', target);
				while(p){
					target = p.target;
					p = jq.tree('getParent', p.target);
				}
				return jq.tree('getNode', target);
			} else {
				var roots = jq.tree('getRoots');
				return roots.length ? roots[0] : null;
			}
		}
	})
})(jQuery);

(function(){
	var plugin = $.fn.slider;
	$.fn.slider = function(options, param){
		var result = plugin.call(this, options, param);
		this.each(function(){
			var state = $.data(this, 'slider');
			if (state.options.disabled){
				state.slider.find('div.slider-inner').unbind('.slider');
			}
		});
		return result;
	};
	$.fn.slider.defaults = plugin.defaults;
	$.fn.slider.parseOptions = plugin.parseOptions;
	$.fn.slider.methods = plugin.methods;
	
	var disableSlider = $.fn.slider.methods.disable;
	$.extend($.fn.slider.methods, {
		disable: function(jq){
			var result = disableSlider.call(this, jq);
			jq.each(function(){
				var state = $.data(this, 'slider');
				state.slider.find('div.slider-inner').unbind('.slider');
			});
			return result;
		}
	});
})(jQuery);

