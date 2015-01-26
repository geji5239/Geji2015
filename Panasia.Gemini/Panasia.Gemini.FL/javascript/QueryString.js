function QueryString(name) {
    var queryString = location.search;
    var Index = queryString.indexOf(name);
    var content = "";
    if (Index != -1) {
        var Index1 = queryString.indexOf("=", Index);
        var Index2 = queryString.indexOf("&", Index1);
        if (Index2 != -1) {
            content = queryString.substring(Index1 + 1, Index2);
        }
        else {
            content = queryString.substring(Index1 + 1);
        }
    }
    return content;
}