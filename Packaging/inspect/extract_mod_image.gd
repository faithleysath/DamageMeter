extends SceneTree


func _init() -> void:
    var args := OS.get_cmdline_user_args()
    if args.size() < 2:
        printerr("need pack path and output path")
        quit(1)
        return

    var pack_path := args[0]
    var output_path := args[1]

    if !ProjectSettings.load_resource_pack(pack_path, true):
        printerr("failed to load pack")
        quit(2)
        return

    var texture := ResourceLoader.load("res://DamageMeter/mod_image.png")
    if texture == null:
        printerr("failed to load res://DamageMeter/mod_image.png")
        quit(3)
        return

    if !texture.has_method("get_image"):
        printerr("texture does not expose get_image")
        quit(4)
        return

    var image: Image = texture.get_image()
    if image == null:
        printerr("failed to decode image")
        quit(5)
        return

    var err := image.save_png(output_path)
    if err != OK:
        printerr("failed to save png: %s" % err)
        quit(err)
        return

    print("saved mod image to %s" % output_path)
    quit(0)
