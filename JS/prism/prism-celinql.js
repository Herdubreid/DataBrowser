Prism.languages.celinql = {
	'string': {
		pattern: /"(?:""|[!#$%&'()*,\/:;<=>?^_ +\-.A-Z\d])*"/i,
		greedy: true
	},
    'italic': /^[fv]\w+/,
    'function': /\b(?:sum|min|max|avg|count|count_distinct|avg_distinct|group|desc|asc)\b/,
    'keyword': /\b(?:all|any)\b/,
    'operator':/>=?|<[=>]?|\b(bw|in)\b|[=?_!^\$]/,
    'punctuation': /[{}[\];(),.:]/,
    'number': /\b\d+\b/
};
