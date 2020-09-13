## Known Bugs

- If you have an Image (or an Element with an Image inside of it) and you don't specify its height and width, then it might outgrow its allowed area (Slide.padding, your literal browser window). In css overflow: hidden; will be set, so it won't completely destroy your presentation. But it'd be better to set both height and width for now.