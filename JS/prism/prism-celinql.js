Prism.languages.celinql = {
	'string': {
		pattern: /"(?:""|[!#$%&'()*,\/:;<=>?^_ +\-.A-Z\d])*"/i,
		greedy: true
	},
    'italic': /^[fv]\w+/,
    'bold': /[[\]()]/,
    'function': /\b(?:sum|min|max|avg|count|count_distinct|avg_distinct|group|desc|asc|demo|v2)\b/,
    'keyword': /\b(?:all|any)\b/,
    'operator':/>=?|<[=>]?|\b(bw|in)\b|[=?_!^\$]/,
    'punctuation': /[{};,.:]/,
    'number': /\b\d+\b/
};
