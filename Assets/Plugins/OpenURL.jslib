mergeInto(LibraryManager.library, {

  OpenURL: function(link)
    {
    	var url = Pointer_stringify(link);
        window.open(url);
    }

});