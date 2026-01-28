using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Custom Rule Tile that supports random animation of tiles.
/// Each tile can play a random animation from the available animated tiles.
/// </summary>
[CreateAssetMenu(fileName = "New Random Animated Rule Tile", menuName = "2D/Tiles/Random Animated Rule Tile")]
public class RandomAnimatedRuleTile : RuleTile
{
    [Header("Random Animation Settings")]
    [Tooltip("Enable random animation for this tile")]
    public bool useRandomAnimation = true;

    [Tooltip("Minimum animation speed multiplier (make larger difference for more noticeable effect)")]
    [Range(0.1f, 5f)]
    public float minAnimationSpeed = 0.5f;

    [Tooltip("Maximum animation speed multiplier (make larger difference for more noticeable effect)")]
    [Range(0.1f, 5f)]
    public float maxAnimationSpeed = 2.0f;

    [Tooltip("Random animation offset for variation - each tile starts at a different frame")]
    public bool randomizeStartFrame = true;
    
    [Tooltip("Use different random seed each time (not position-based) - tiles will vary on reload")]
    public bool useTimeBasedSeed = false;
    
    [Tooltip("Randomly pick from all available animation sets in the rule tile, ignoring rule matching")]
    public bool randomizeAnimationSet = true;

    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        // Initialize random seed first
        if (useTimeBasedSeed)
        {
            Random.InitState(System.Environment.TickCount + position.x + position.y + position.z);
        }
        else
        {
            Random.InitState(position.x * 73856093 ^ position.y * 19349663 ^ position.z * 83492791);
        }

        // If randomize animation set is enabled, pick a random animation from all rules
        if (randomizeAnimationSet && m_TilingRules != null && m_TilingRules.Count > 0)
        {
            // Collect all rules that have animations
            System.Collections.Generic.List<TilingRule> animatedRules = new System.Collections.Generic.List<TilingRule>();
            foreach (var rule in m_TilingRules)
            {
                if (rule.m_Sprites != null && rule.m_Sprites.Length > 1 && rule.m_Output == TilingRuleOutput.OutputSprite.Animation)
                {
                    animatedRules.Add(rule);
                }
            }

            // Pick a random animated rule
            if (animatedRules.Count > 0)
            {
                TilingRule randomRule = animatedRules[Random.Range(0, animatedRules.Count)];
                
                tileAnimationData.animatedSprites = randomRule.m_Sprites;
                tileAnimationData.animationSpeed = Random.Range(randomRule.m_MinAnimationSpeed, randomRule.m_MaxAnimationSpeed);
                
                if (useRandomAnimation)
                {
                    // Randomize animation speed
                    float randomSpeed = Random.Range(minAnimationSpeed, maxAnimationSpeed);
                    tileAnimationData.animationSpeed *= randomSpeed;

                    // Randomize start frame
                    if (randomizeStartFrame && tileAnimationData.animatedSprites.Length > 1)
                    {
                        int randomFrame = Random.Range(0, tileAnimationData.animatedSprites.Length);
                        tileAnimationData.animationStartTime = randomFrame / tileAnimationData.animationSpeed;
                    }
                }
                
                return true;
            }
        }

        // Fall back to default behavior
        if (!useRandomAnimation)
        {
            return base.GetTileAnimationData(position, tilemap, ref tileAnimationData);
        }

        bool hasAnimation = base.GetTileAnimationData(position, tilemap, ref tileAnimationData);

        if (hasAnimation && tileAnimationData.animatedSprites != null && tileAnimationData.animatedSprites.Length > 0)
        {
            // Randomize animation speed with more noticeable variation
            float randomSpeed = Random.Range(minAnimationSpeed, maxAnimationSpeed);
            tileAnimationData.animationSpeed = tileAnimationData.animationSpeed * randomSpeed;

            // Randomize start frame if enabled - this makes tiles appear out of sync
            if (randomizeStartFrame && tileAnimationData.animatedSprites.Length > 1)
            {
                // Pick a random frame to start from
                int randomFrame = Random.Range(0, tileAnimationData.animatedSprites.Length);
                tileAnimationData.animationStartTime = randomFrame / tileAnimationData.animationSpeed;
            }

            return true;
        }

        return hasAnimation;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
    }
}
