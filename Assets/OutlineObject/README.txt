//
// Outline Object
// Setup/Usage Instructions
//

For a visual representation of the following, please refer to the video on the
plugin page in the Asset Store.

See also a list of known limitations regarding this plugin and descriptions of
available public methods at the bottom of this document.

----------

[Release Notes]

v1.1 (05/14/18) => Added spritesheet, animation, and isometric sorting support.

v1.0 (01/02/18) => Initial release.

----------

[Setup]

The only setup step required for using this plugin is:
 * turning on "Read/Write Enabled" in the Import Settings of *every* sprite
   that requires an outline.

Q: Why is this necessary?
A: In order for the included pixel shader to display outlines around your
   sprites, each sprite must have a border of transparent pixels surrounding it
   that is equal or greater to the desired size of the outline. For example, if
   a 5px outline is desired, the sprite must have a 5px border of transparent
   pixels surrounding it in all 4 directions (above/below and both sides).

   Rather than having to manually add the aforementioned border to each and
   every sprite that requires an outline, this plugin does it for you
   automatically via code (by creating a new texture in memory; it does not
   modify the original source texture). "Read/Write Enabled" must be checked to
   allow the raw pixel data of the sprite to be accessed.

----------

[Usage]

1. Add the "Outline Object" component on all parent sprites that require an
   outline. (Refer to steps 6-8 for important information about child sprites).

2. Set "Outline Material" to "Sprites-Outline" to allow the generated outline
   sprite to be rendered using the outline shader.

3. Use "Outline Color" to change the color of the outline from white to a color
   of your choosing.

4. Drag the "Outline Size" slider to adjust the size of the outline anywhere
   from 1px up to 10px thick.

5. Drag the "Outline Blur" slider to blur the outline by adding anti-aliasing
   to the outer edges of the outline.

6. Enable "Include Children" to have child sprites included in the outline.

7. Adjust "Child Layers" to filter which child sprites will be included in the
   outline. This is useful if you have UI sprites attached to a sprite which
   you do not want to be included in the outline.

8. Enable "Children Overlap" if any two or more sprites with transparent pixels
   in them overlap each other to ensure that the transparent pixels do not mask
   out the opaque ones in the other sprites -- potentially removing portions of
   the outline. This includes child sprites that overlap the main parent
   sprite as well. (Refer to the video at 1:49 to see a visual.)

9. Enable "Generates On Start" to have outlines auto-generated at game start.
   This also disables real-time changes while in edit mode.

10. Enable "Is Animated" to have outlines automatically regenerated when the
    main parent sprite frame changes.

11. Enable "Is Isometric" to have outlines sorted using their Z-axis position
    instead of sorting order.

----------

[Known Limitations]

* Rotation/scaling is only supported on the main parent sprite, not children.
  If you need to rotate or scale a child sprite that requires an outline, you
  should instead add individual "Outline Object" components to each sprite with
  the "Include Children" option set to false.

* Child sprites aren't included in the outline if they are nested within a game
  object that does not have a Sprite Renderer attached. For example, take the
  following prefab structure:

   Hero (has Sprite Renderer & Outline Object components)
    - Weapon Slot (used to offset attached bow, does not have Sprite Renderer)
      - Bow (has Sprite Renderer)

  In this scenario, "Bow" will not show up in the outline as "Weapon Slot" does
  not contain a Sprite Renderer and the Outline Object component will not
  traverse the children of that game object.

----------

[Public Methods]

* Regenerate() -> Updates the outline.

* Clear() -> Permanently destroys the outline.

* Hide() -> Makes the outline disappear, without permanently destroying it.

* Show() -> Makes the outline visible again.

* SetSortOrder() -> Sets the sort order of the outline to the specified value.

* SetSortOrderOffset() -> Sets the sort order of the outline to the lowest sort
                          order of all included sprites, offsetted by the
                          specified value.

* ShouldIgnoreSprite() [Overridable] -> Tells the component which child sprites
                                        to *not* include in the outline when
                                        traversing the children of the main
                                        parent sprite.

----------

Thanks for reading!

Have questions? Send an email to: support@legit-games.com


